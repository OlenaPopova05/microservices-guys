namespace CoreService.Application.Commands;

public record CreateHabitCommand(Guid OwnerUserId, string Title, string? Description);
