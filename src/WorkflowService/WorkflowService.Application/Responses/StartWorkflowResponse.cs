using WorkflowService.Domain;

namespace WorkflowService.Application.Responses;

public record StartWorkflowResponse(
    Guid WorkflowId,
    WorkflowState State,
    Guid? HabitId,
    string? Message);
