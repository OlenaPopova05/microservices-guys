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
        using var response = await _httpClient.GetAsync($"/users/{userId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new BadRequestException("Owner user not found.");

        if (!response.IsSuccessStatusCode)
        {
            var message = await TryReadMessageAsync(response, cancellationToken)
                          ?? $"Users service returned {(int)response.StatusCode}.";
            throw new InvalidOperationException(message);
        }
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
