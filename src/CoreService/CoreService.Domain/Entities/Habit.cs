namespace CoreService.Domain;

public class Habit
{
    public Guid Id { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public HabitStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private Habit() { }

    public Habit(Guid ownerUserId, string title, string? description)
    {
        if (ownerUserId == Guid.Empty)
            throw new ArgumentException("OwnerUserId is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");

        Id = Guid.NewGuid();
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Status = HabitStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void UpdateStatus(HabitStatus newStatus)
    {
        if (Status == HabitStatus.Archived)
            throw new InvalidOperationException("Archived habit cannot be modified.");

        if (Status == HabitStatus.Completed && newStatus == HabitStatus.Active)
            throw new InvalidOperationException("Completed habit cannot be moved back to active.");

        if (newStatus == HabitStatus.Archived && Status != HabitStatus.Completed)
            throw new InvalidOperationException("Only completed habit can be archived.");

        if (Status == newStatus)
            return;

        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}