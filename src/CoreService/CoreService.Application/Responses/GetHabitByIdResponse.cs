using CoreService.Domain;

namespace CoreService.Application.Responses;

public record GetHabitByIdResponse(
    Guid HabitId,
    Guid OwnerUserId,
    string Title,
    string? Description,
    HabitStatus Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
