using CoreService.Domain;

namespace CoreService.Application.Interfaces;

public interface IHabitRepository
{
    Task<Habit> CreateAsync(Habit habit);
    Task<Habit?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}
