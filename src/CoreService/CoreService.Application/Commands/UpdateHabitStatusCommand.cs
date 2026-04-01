using CoreService.Domain;

namespace CoreService.Application.Commands;

public record UpdateHabitStatusCommand(Guid HabitId, HabitStatus Status);
