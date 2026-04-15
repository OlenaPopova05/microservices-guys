using WorkflowService.Api.Middleware;
using WorkflowService.Api.Requests;
using WorkflowService.Application.Commands;
using WorkflowService.Application.Handlers;
using WorkflowService.Application.Interfaces;
using WorkflowService.Infrastructure.Clients;
using WorkflowService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddHttpClient<IUsersServiceClient, UsersServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UsersService:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ICoreServiceClient, CoreServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CoreService:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
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
