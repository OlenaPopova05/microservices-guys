using CoreService.Api.Requests;
using CoreService.Application.Commands;
using CoreService.Application.Exceptions;
using CoreService.Application.Handlers;
using CoreService.Application.Queries;
using CoreService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Api.Controllers;

[ApiController]
[Route("habits")]
public class HabitsController : ControllerBase
{
    private readonly CreateHabitHandler _createHabitHandler;
    private readonly GetHabitByIdHandler _getHabitByIdHandler;
    private readonly UpdateHabitStatusHandler _updateHabitStatusHandler;

    public HabitsController(
        CreateHabitHandler createHabitHandler,
        GetHabitByIdHandler getHabitByIdHandler,
        UpdateHabitStatusHandler updateHabitStatusHandler)
    {
        _createHabitHandler = createHabitHandler;
        _getHabitByIdHandler = getHabitByIdHandler;
        _updateHabitStatusHandler = updateHabitStatusHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHabitRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateHabitCommand(request.OwnerUserId, request.Title, request.Description);
        var result = await _createHabitHandler.Handle(command, cancellationToken);
        return Created($"/habits/{result.HabitId}", result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetHabitByIdQuery(id);
        var result = await _getHabitByIdHandler.Handle(query);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateHabitStatusRequest request)
    {
        if (!Enum.TryParse<HabitStatus>(request.Status, true, out var status))
            throw new BadRequestException("Invalid habit status.");

        var command = new UpdateHabitStatusCommand(id, status);
        var result = await _updateHabitStatusHandler.Handle(command);
        return Ok(result);
    }
}