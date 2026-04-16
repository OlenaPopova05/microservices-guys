using CoreService.Application.Interfaces;
using CoreService.Domain;

namespace CoreService.Infrastructure;

public class HabitRepository : IHabitRepository
{
    private readonly CoreDbContext _context;

    public HabitRepository(CoreDbContext context)
    {
        _context = context;
    }

    public async Task<Habit> CreateAsync(Habit habit)
    {
        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();
        return habit;
    }

    public Task<Habit?> GetByIdAsync(Guid id)
    {
        return _context.Habits.FindAsync(id).AsTask();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var habit = await _context.Habits.FindAsync([id], cancellationToken);
        if (habit is null)
            return false;

        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}