using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Api;
using NotificationService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NotificationsDb")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HabitCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]!, "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("notification-habit-created", e =>
        {
            e.ConfigureConsumer<HabitCreatedConsumer>(context);
        });
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();