namespace WorkflowService.Application.Interfaces;

public interface ICoreServiceClient
{
    Task<Guid> CreateHabitAsync(Guid ownerUserId, string title, string? description, CancellationToken cancellationToken = default);
    Task UpdateHabitStatusAsync(Guid habitId, string status, CancellationToken cancellationToken = default);
    Task DeleteHabitAsync(Guid habitId, CancellationToken cancellationToken = default);
}
