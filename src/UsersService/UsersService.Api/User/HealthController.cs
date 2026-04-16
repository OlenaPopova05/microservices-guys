using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersService.Infrastructure;

namespace UsersService.Api.User;

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
    public async Task<IActionResult> Readiness([FromServices] UsersDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);

        return Ok(new
        {
            status = dbHealthy ? "Healthy" : "Unhealthy",
            database = dbHealthy ? "Healthy" : "Unhealthy"
        });
    }
}