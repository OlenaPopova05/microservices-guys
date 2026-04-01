using UsersService.Application.Interfaces;
using UsersService.Application.Commands;
using UsersService.Application.Responses;
using UsersService.Domain;
using UsersService.Application.Exceptions;

namespace UsersService.Application.Handlers;

public class CreateUserHandler
{
    private readonly IUserRepository _repository;

    public CreateUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CreateUserResponse> Handle(CreateUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.DisplayName))
            throw new BadRequestException("Display name is required");
        
        var user = new User(command.DisplayName);
 
        await _repository.CreateAsync(user);

        return new CreateUserResponse(user.Id);
    }
}