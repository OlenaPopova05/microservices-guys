using WorkflowService.Api.Requests;
using WorkflowService.Application.Commands;
using WorkflowService.Application.Exceptions;
using WorkflowService.Application.Handlers;
using WorkflowService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WorkflowService.Api.Controllers;

[ApiController]
[Route("workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly StartWorkflowHandler _startWorkflowHandler;
    private readonly GetWorkflowByIdHandler _getWorkflowByIdHandler;

    public WorkflowsController(StartWorkflowHandler startWorkflowHandler, GetWorkflowByIdHandler getWorkflowByIdHandler)
    {
        _startWorkflowHandler = startWorkflowHandler;
        _getWorkflowByIdHandler = getWorkflowByIdHandler;
    }

    [HttpPost("{action}")]
    public async Task<IActionResult> Start(string action, [FromBody] StartWorkflowRequest request, CancellationToken cancellationToken)
    {
        var command = new StartWorkflowCommand(
            action,
            request.OwnerUserId,
            request.Title,
            request.Description,
            request.ForceStep3Failure);

        var result = await _startWorkflowHandler.Handle(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{workflowId:guid}")]
    public async Task<IActionResult> GetById(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _getWorkflowByIdHandler.Handle(new GetWorkflowByIdQuery(workflowId), cancellationToken);
        if (result is null)
            throw new NotFoundException("Workflow not found.");

        return Ok(result);
    }
}
