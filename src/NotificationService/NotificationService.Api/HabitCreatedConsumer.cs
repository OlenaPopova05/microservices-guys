using System.Text.Json;
using CoreService.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure;

namespace NotificationService.Api;

public class HabitCreatedConsumer : IConsumer<HabitCreatedEvent>
{
    private readonly NotificationsDbContext _dbContext;
    private readonly ILogger<HabitCreatedConsumer> _logger;

    public HabitCreatedConsumer(NotificationsDbContext dbContext, ILogger<HabitCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HabitCreatedEvent> context)
    {
        var message = context.Message;

        var notification = new NotificationEntity
        {
            EventId = message.EventId,
            CorrelationId = message.CorrelationId,
            HabitId = message.HabitId,
            OwnerUserId = message.OwnerUserId,
            Summary = message.Summary,
            OccurredAt = message.OccurredAt,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);

        try
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Notification stored for event {EventId}", message.EventId);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Duplicate event ignored: {EventId}", message.EventId);
        }
    }
}