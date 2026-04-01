namespace NotificationService.Infrastructure;

public class NotificationEntity
{
    public Guid EventId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public Guid HabitId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}