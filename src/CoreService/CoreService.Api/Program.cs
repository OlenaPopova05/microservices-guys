using CoreService.Api.Middleware;
using CoreService.Api.Options;
using CoreService.Application.Handlers;
using CoreService.Application.Interfaces;
using CoreService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CoreDb")));

builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<CreateHabitHandler>();
builder.Services.AddScoped<GetHabitByIdHandler>();
builder.Services.AddScoped<UpdateHabitStatusHandler>();
builder.Services.AddScoped<DeleteHabitHandler>();

builder.Services.Configure<UsersServiceOptions>(builder.Configuration.GetSection("UsersService"));

var usersServiceResilience = builder.Configuration.GetSection("UsersService").Get<UsersServiceOptions>()?.Resilience
    ?? new HttpResilienceOptions();

builder.Services.AddHttpClient<IUsersServiceClient, UsersServiceClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<UsersServiceOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = Timeout.InfiniteTimeSpan;
    })
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = usersServiceResilience.MaxRetryAttempts;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(usersServiceResilience.AttemptTimeoutSeconds);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(usersServiceResilience.TotalRequestTimeoutSeconds);
    });

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Password"]!);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();