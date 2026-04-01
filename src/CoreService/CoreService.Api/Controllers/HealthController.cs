using CoreService.Application.Interfaces;
using CoreService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet("live")]
    public IActionResult Liveness()
    {
        return Ok(new { status = "Healthy" });
    }

    [HttpGet]
    public async Task<IActionResult> Readiness(
        [FromServices] CoreDbContext dbContext,
        [FromServices] IUsersServiceClient usersServiceClient,
        CancellationToken cancellationToken)
    {
        var dbHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);
        var usersHealthy = await usersServiceClient.IsHealthyAsync(cancellationToken);
        var overallHealthy = dbHealthy && usersHealthy;

        return Ok(new
        {
            status = overallHealthy ? "Healthy" : "Unhealthy",
            database = dbHealthy ? "Healthy" : "Unhealthy",
            usersService = usersHealthy ? "Healthy" : "Unhealthy"
        });
    }
}
