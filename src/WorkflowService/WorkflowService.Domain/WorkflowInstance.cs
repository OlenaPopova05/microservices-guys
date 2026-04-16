namespace WorkflowService.Domain;

public class WorkflowInstance
{
    public Guid WorkflowId { get; private set; }
    public string Type { get; private set; } = null!;
    public WorkflowState State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? LastError { get; private set; }

    private WorkflowInstance() { }

    public WorkflowInstance(Guid workflowId, string type)
    {
        if (workflowId == Guid.Empty)
            throw new ArgumentException("WorkflowId is required.", nameof(workflowId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type is required.", nameof(type));

        WorkflowId = workflowId;
        Type = type.Trim();
        State = WorkflowState.Started;
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void SetState(WorkflowState state, string? lastError = null)
    {
        State = state;
        LastError = lastError;
        UpdatedAt = DateTime.UtcNow;
    }
}
