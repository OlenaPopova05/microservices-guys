using UsersService.Domain;
using UsersService.Application.Interfaces;

namespace UsersService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        // FindAsync returns ValueTask so we should use AsTask()
        return _context.Users.FindAsync(id).AsTask();
    }
}