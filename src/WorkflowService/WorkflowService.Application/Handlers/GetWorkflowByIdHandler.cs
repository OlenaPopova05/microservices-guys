using WorkflowService.Application.Interfaces;
using WorkflowService.Application.Queries;
using WorkflowService.Application.Responses;

namespace WorkflowService.Application.Handlers;

public class GetWorkflowByIdHandler
{
    private readonly IWorkflowRepository _workflowRepository;

    public GetWorkflowByIdHandler(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<WorkflowStatusResponse?> Handle(GetWorkflowByIdQuery query, CancellationToken cancellationToken = default)
    {
        var entity = await _workflowRepository.GetByIdAsync(query.WorkflowId, cancellationToken);
        if (entity is null)
            return null;

        return new WorkflowStatusResponse(
            entity.WorkflowId,
            entity.Type,
            entity.State,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.LastError);
    }
}
