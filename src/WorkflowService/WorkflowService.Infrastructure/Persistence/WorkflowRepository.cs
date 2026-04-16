using Microsoft.EntityFrameworkCore;
using WorkflowService.Application.Interfaces;
using WorkflowService.Domain;

namespace WorkflowService.Infrastructure.Persistence;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly WorkflowDbContext _dbContext;

    public WorkflowRepository(WorkflowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkflowInstances.AddAsync(instance, cancellationToken);
    }

    public Task<WorkflowInstance?> GetByIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        return _dbContext.WorkflowInstances.FirstOrDefaultAsync(x => x.WorkflowId == workflowId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
