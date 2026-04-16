namespace WorkflowService.Application.Interfaces;

public interface IUsersServiceClient
{
    Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
