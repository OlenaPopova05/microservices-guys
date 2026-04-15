using WorkflowService.Application.Commands;
using WorkflowService.Application.Exceptions;
using WorkflowService.Application.Interfaces;
using WorkflowService.Application.Responses;
using WorkflowService.Domain;

namespace WorkflowService.Application.Handlers;

public class StartWorkflowHandler
{
    public const string CreateHabitAction = "create-habit";

    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly ICoreServiceClient _coreServiceClient;

    public StartWorkflowHandler(
        IWorkflowRepository workflowRepository,
        IUsersServiceClient usersServiceClient,
        ICoreServiceClient coreServiceClient)
    {
        _workflowRepository = workflowRepository;
        _usersServiceClient = usersServiceClient;
        _coreServiceClient = coreServiceClient;
    }

    public async Task<StartWorkflowResponse> Handle(StartWorkflowCommand command, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(command.Action, CreateHabitAction, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException($"Unknown workflow action '{command.Action}'. Supported: {CreateHabitAction}.");

        if (command.OwnerUserId == Guid.Empty)
            throw new BadRequestException("OwnerUserId is required.");

        if (string.IsNullOrWhiteSpace(command.Title))
            throw new BadRequestException("Title is required.");

        var workflowId = Guid.NewGuid();
        var instance = new WorkflowInstance(workflowId, CreateHabitAction);
        await _workflowRepository.AddAsync(instance, cancellationToken);
        await _workflowRepository.SaveChangesAsync(cancellationToken);

        Guid? habitId = null;

        try
        {
            await _usersServiceClient.EnsureUserExistsAsync(command.OwnerUserId, cancellationToken);
            instance.SetState(WorkflowState.UserValidated);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            habitId = await _coreServiceClient.CreateHabitAsync(
                command.OwnerUserId,
                command.Title,
                command.Description,
                cancellationToken);

            instance.SetState(WorkflowState.HabitCreated);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            instance.SetState(WorkflowState.CompletingHabit);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            if (command.ForceStep3Failure)
                throw new InvalidOperationException(
                    "Simulated step 3 failure (forceStep3Failure). CoreService PATCH was skipped to demonstrate compensation.");

            await _coreServiceClient.UpdateHabitStatusAsync(habitId.Value, "Completed", cancellationToken);

            instance.SetState(WorkflowState.Completed);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            return new StartWorkflowResponse(workflowId, WorkflowState.Completed, habitId, null);
        }
        catch (BadRequestException ex)
        {
            instance.SetState(WorkflowState.Failed, ex.Message);
            await _workflowRepository.SaveChangesAsync(cancellationToken);
            throw;
        }
        catch (Exception ex) when (habitId is not null)
        {
            await CompensateAsync(instance, habitId.Value, ex, cancellationToken);
            return new StartWorkflowResponse(
                workflowId,
                WorkflowState.Compensated,
                null,
                "Workflow compensated: habit was deleted after a downstream failure.");
        }
        catch (Exception ex)
        {
            instance.SetState(WorkflowState.Failed, ex.Message);
            await _workflowRepository.SaveChangesAsync(cancellationToken);
            return new StartWorkflowResponse(workflowId, WorkflowState.Failed, null, ex.Message);
        }
    }

    private async Task CompensateAsync(
        WorkflowInstance instance,
        Guid habitId,
        Exception failure,
        CancellationToken cancellationToken)
    {
        instance.SetState(WorkflowState.Compensating, failure.Message);
        await _workflowRepository.SaveChangesAsync(cancellationToken);

        try
        {
            await _coreServiceClient.DeleteHabitAsync(habitId, cancellationToken);
            instance.SetState(WorkflowState.Compensated, failure.Message);
        }
        catch (Exception ex)
        {
            instance.SetState(
                WorkflowState.Failed,
                $"Compensation failed after: {failure.Message}. Delete habit error: {ex.Message}");
        }

        await _workflowRepository.SaveChangesAsync(cancellationToken);
    }
}
