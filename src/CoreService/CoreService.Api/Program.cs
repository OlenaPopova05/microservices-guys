using CoreService.Api.Middleware;
using CoreService.Application.Handlers;
using CoreService.Application.Interfaces;
using CoreService.Infrastructure;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddHttpClient<IUsersServiceClient, UsersServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UsersService:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(3);
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