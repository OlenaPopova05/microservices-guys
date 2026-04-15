using CoreService.Application.Commands;
using CoreService.Application.Exceptions;
using CoreService.Application.Interfaces;

namespace CoreService.Application.Handlers;

public class DeleteHabitHandler
{
    private readonly IHabitRepository _habitRepository;

    public DeleteHabitHandler(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task Handle(DeleteHabitCommand command, CancellationToken cancellationToken = default)
    {
        if (command.HabitId == Guid.Empty)
            throw new BadRequestException("HabitId is required.");

        var deleted = await _habitRepository.DeleteAsync(command.HabitId, cancellationToken);
        if (!deleted)
            throw new NotFoundException("Habit not found.");
    }
}
