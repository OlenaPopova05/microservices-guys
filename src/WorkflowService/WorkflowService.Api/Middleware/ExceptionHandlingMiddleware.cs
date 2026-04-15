using System.Net;
using System.Text.Json;
using WorkflowService.Application.Exceptions;

namespace WorkflowService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException ex)
        {
            await WriteJsonAsync(context, HttpStatusCode.BadRequest, "bad_request", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteJsonAsync(context, HttpStatusCode.NotFound, "not_found", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            await WriteJsonAsync(context, HttpStatusCode.InternalServerError, "internal_server_error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteJsonAsync(HttpContext context, HttpStatusCode statusCode, string error, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new
        {
            error,
            message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}