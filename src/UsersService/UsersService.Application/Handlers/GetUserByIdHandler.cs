using UsersService.Application.Interfaces;
using UsersService.Application.Queries;
using UsersService.Application.Responses;
using UsersService.Application.Exceptions;

namespace UsersService.Application.Handlers;

public class GetUserByIdHandler
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery query)
    {
        if (query.UserId == Guid.Empty)
            throw new BadRequestException("User id is required");
        
        var user = await _repository.GetByIdAsync(query.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        return new GetUserByIdResponse(user.Id, user.DisplayName);
    }
}