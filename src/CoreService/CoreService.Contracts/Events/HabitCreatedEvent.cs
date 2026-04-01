namespace CoreService.Contracts.Events;

public class HabitCreatedEvent
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public Guid HabitId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Summary { get; set; } = string.Empty;
}