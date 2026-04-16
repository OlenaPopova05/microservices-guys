using CoreService.Domain;

namespace CoreService.Application.Responses;

public record CreateHabitResponse(Guid HabitId, Guid OwnerUserId, string Title, string? Description, HabitStatus Status);
