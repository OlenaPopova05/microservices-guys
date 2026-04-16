using UsersService.Application.Commands;
using UsersService.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Handlers;

namespace UsersService.Api.User;
 
[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly CreateUserHandler _createUserHandler;
    private readonly GetUserByIdHandler _getUserByIdHandler;
    
    public UsersController(CreateUserHandler createUserHandler, GetUserByIdHandler getUserByIdHandler)
    {
        _createUserHandler = createUserHandler;
        _getUserByIdHandler = getUserByIdHandler;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.DisplayName);
        var result = await _createUserHandler.Handle(command);

        return Created($"/users/{result.UserId}", result);

    }
 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _getUserByIdHandler.Handle(query);

        return Ok(result);
    }
}