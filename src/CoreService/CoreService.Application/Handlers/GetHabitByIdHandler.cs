using CoreService.Application.Exceptions;
using CoreService.Application.Interfaces;
using CoreService.Application.Queries;
using CoreService.Application.Responses;

namespace CoreService.Application.Handlers;

public class GetHabitByIdHandler
{
    private readonly IHabitRepository _habitRepository;

    public GetHabitByIdHandler(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task<GetHabitByIdResponse> Handle(GetHabitByIdQuery query)
    {
        if (query.HabitId == Guid.Empty)
            throw new BadRequestException("HabitId is required.");

        var habit = await _habitRepository.GetByIdAsync(query.HabitId);
        if (habit is null)
            throw new NotFoundException("Habit not found.");

        return new GetHabitByIdResponse(
            habit.Id,
            habit.OwnerUserId,
            habit.Title,
            habit.Description,
            habit.Status,
            habit.CreatedAtUtc,
            habit.UpdatedAtUtc);
    }
}
