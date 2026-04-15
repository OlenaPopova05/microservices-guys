using WorkflowService.Domain;

namespace WorkflowService.Application.Responses;

public record WorkflowStatusResponse(
    Guid WorkflowId,
    string Type,
    WorkflowState State,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? LastError);
