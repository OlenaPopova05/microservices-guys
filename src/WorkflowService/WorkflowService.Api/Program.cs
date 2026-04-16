using WorkflowService.Api.Middleware;
using WorkflowService.Api.Options;
using WorkflowService.Api.Requests;
using WorkflowService.Application.Commands;
using WorkflowService.Application.Handlers;
using WorkflowService.Application.Interfaces;
using WorkflowService.Infrastructure.Clients;
using WorkflowService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WorkflowDb")));

builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<StartWorkflowHandler>();
builder.Services.AddScoped<GetWorkflowByIdHandler>();

builder.Services.Configure<RemoteServiceEndpointOptions>("Users", builder.Configuration.GetSection("UsersService"));
builder.Services.Configure<RemoteServiceEndpointOptions>("Core", builder.Configuration.GetSection("CoreService"));

var usersDownstreamResilience = builder.Configuration.GetSection("UsersService").Get<RemoteServiceEndpointOptions>()?.Resilience
    ?? new HttpResilienceOptions();
var coreDownstreamResilience = builder.Configuration.GetSection("CoreService").Get<RemoteServiceEndpointOptions>()?.Resilience
    ?? new HttpResilienceOptions();

builder.Services.AddHttpClient<IUsersServiceClient, UsersServiceClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptionsMonitor<RemoteServiceEndpointOptions>>().Get("Users");
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = Timeout.InfiniteTimeSpan;
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = usersDownstreamResilience.MaxRetryAttempts;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(usersDownstreamResilience.AttemptTimeoutSeconds);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(usersDownstreamResilience.TotalRequestTimeoutSeconds);
    });

builder.Services.AddHttpClient<ICoreServiceClient, CoreServiceClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptionsMonitor<RemoteServiceEndpointOptions>>().Get("Core");
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = Timeout.InfiniteTimeSpan;
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = coreDownstreamResilience.MaxRetryAttempts;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(coreDownstreamResilience.AttemptTimeoutSeconds);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(coreDownstreamResilience.TotalRequestTimeoutSeconds);
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapControllers();
await app.RunAsync();
