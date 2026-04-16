namespace WorkflowService.Application.Commands;

public record StartWorkflowCommand(
    string Action,
    Guid OwnerUserId,
    string Title,
    string? Description,
    bool ForceStep3Failure);
