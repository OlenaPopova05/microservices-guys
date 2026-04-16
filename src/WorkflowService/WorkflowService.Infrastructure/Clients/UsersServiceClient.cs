using System.Net;
using System.Text.Json;
using WorkflowService.Application.Exceptions;
using WorkflowService.Application.Interfaces;

namespace WorkflowService.Infrastructure.Clients;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly HttpClient _httpClient;

    public UsersServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync($"/users/{userId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new BadRequestException("Owner user not found.");

            if (!response.IsSuccessStatusCode)
            {
                var message = await TryReadMessageAsync(response, cancellationToken);
                ThrowForFailedResponse("Users service", response.StatusCode, message);
            }
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (ServiceUnavailableException)
        {
            throw;
        }
        catch (DependencyTimeoutException)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            throw new ServiceUnavailableException("Users service is unavailable.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new DependencyTimeoutException("Users service request timed out.");
        }
    }

    private static void ThrowForFailedResponse(string dependencyName, HttpStatusCode statusCode, string? detail)
    {
        if (statusCode is HttpStatusCode.GatewayTimeout or HttpStatusCode.RequestTimeout)
            throw new DependencyTimeoutException(
                string.IsNullOrWhiteSpace(detail)
                    ? $"{dependencyName} returned {(int)statusCode}."
                    : $"{dependencyName} returned {(int)statusCode}: {detail}");

        if ((int)statusCode >= 500 || statusCode == HttpStatusCode.BadGateway)
            throw new ServiceUnavailableException(
                string.IsNullOrWhiteSpace(detail)
                    ? $"{dependencyName} returned {(int)statusCode}."
                    : $"{dependencyName} returned {(int)statusCode}: {detail}");

        throw new InvalidOperationException(
            detail ?? $"{dependencyName} returned {(int)statusCode}.");
    }

    private static async Task<string?> TryReadMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString();
        }
        catch
        {
            // ignored
        }

        return null;
    }
}
