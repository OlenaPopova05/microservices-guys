using WorkflowService.Domain;

namespace WorkflowService.Application.Interfaces;

public interface IWorkflowRepository
{
    Task AddAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);
    Task<WorkflowInstance?> GetByIdAsync(Guid workflowId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
