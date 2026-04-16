using CoreService.Domain;

namespace CoreService.Application.Responses;

public record UpdateHabitStatusResponse(Guid HabitId, HabitStatus Status, DateTime UpdatedAtUtc);
