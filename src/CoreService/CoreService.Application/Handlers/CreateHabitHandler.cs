using CoreService.Application.Commands;
using CoreService.Application.Exceptions;
using CoreService.Application.Interfaces;
using CoreService.Application.Responses;
using CoreService.Domain;
using CoreService.Contracts.Events;
using MassTransit;

namespace CoreService.Application.Handlers;

public class CreateHabitHandler
{
    private readonly IHabitRepository _habitRepository;
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateHabitHandler(IHabitRepository habitRepository, IUsersServiceClient usersServiceClient, IPublishEndpoint publishEndpoint)
    {
        _habitRepository = habitRepository;
        _usersServiceClient = usersServiceClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateHabitResponse> Handle(CreateHabitCommand command, CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty)
            throw new BadRequestException("OwnerUserId is required.");

        if (string.IsNullOrWhiteSpace(command.Title))
            throw new BadRequestException("Title is required.");

        var ownerExists = await _usersServiceClient.UserExistsAsync(command.OwnerUserId, cancellationToken);
        if (!ownerExists)
            throw new BadRequestException("Owner user not found.");

        Habit habit;
        try
        {
            habit = new Habit(command.OwnerUserId, command.Title, command.Description);
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException(ex.Message);
        }

        await _habitRepository.CreateAsync(habit);
        
        await _publishEndpoint.Publish(new HabitCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString(),
            HabitId = habit.Id,
            OwnerUserId = habit.OwnerUserId,
            Summary = habit.Title
        }, cancellationToken);

        return new CreateHabitResponse(
            habit.Id,
            habit.OwnerUserId,
            habit.Title,
            habit.Description,
            habit.Status);
    }
}
