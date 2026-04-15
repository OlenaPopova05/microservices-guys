namespace WorkflowService.Domain;

public enum WorkflowState
{
    Started = 0,
    UserValidated = 1,
    HabitCreated = 2,
    CompletingHabit = 3,
    Completed = 4,
    Compensating = 5,
    Compensated = 6,
    Failed = 7
}
