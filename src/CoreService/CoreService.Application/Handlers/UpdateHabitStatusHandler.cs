using CoreService.Application.Commands;
using CoreService.Application.Exceptions;
using CoreService.Application.Interfaces;
using CoreService.Application.Responses;

namespace CoreService.Application.Handlers;

public class UpdateHabitStatusHandler
{
    private readonly IHabitRepository _habitRepository;

    public UpdateHabitStatusHandler(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task<UpdateHabitStatusResponse> Handle(UpdateHabitStatusCommand command)
    {
        if (command.HabitId == Guid.Empty)
            throw new BadRequestException("HabitId is required.");

        var habit = await _habitRepository.GetByIdAsync(command.HabitId);
        if (habit is null)
            throw new NotFoundException("Habit not found.");

        try
        {
            habit.UpdateStatus(command.Status);
        }
        catch (InvalidOperationException ex)
        {
            throw new BadRequestException(ex.Message);
        }

        await _habitRepository.SaveChangesAsync();

        return new UpdateHabitStatusResponse(habit.Id, habit.Status, habit.UpdatedAtUtc);
    }
}
