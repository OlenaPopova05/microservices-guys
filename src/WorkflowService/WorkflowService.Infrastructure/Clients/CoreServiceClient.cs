using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorkflowService.Application.Interfaces;

namespace WorkflowService.Infrastructure.Clients;

public class CoreServiceClient : ICoreServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;

    public CoreServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid> CreateHabitAsync(
        Guid ownerUserId,
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var payload = new { ownerUserId, title, description };
        using var response = await _httpClient.PostAsJsonAsync("/habits", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await TryReadMessageAsync(response, cancellationToken)
                          ?? $"Core service returned {(int)response.StatusCode} while creating habit.";
            throw new InvalidOperationException(message);
        }

        var body = await response.Content.ReadFromJsonAsync<CreateHabitResponseDto>(JsonOptions, cancellationToken);
        if (body?.HabitId is null || body.HabitId == Guid.Empty)
            throw new InvalidOperationException("Core service returned an unexpected create habit response.");

        return body.HabitId.Value;
    }

    public async Task UpdateHabitStatusAsync(Guid habitId, string status, CancellationToken cancellationToken = default)
    {
        var payload = new { status };
        using var response = await _httpClient.PatchAsJsonAsync($"/habits/{habitId}/status", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await TryReadMessageAsync(response, cancellationToken)
                          ?? $"Core service returned {(int)response.StatusCode} while updating habit status.";
            throw new InvalidOperationException(message);
        }
    }

    public async Task DeleteHabitAsync(Guid habitId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"/habits/{habitId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return;

        if (!response.IsSuccessStatusCode)
        {
            var message = await TryReadMessageAsync(response, cancellationToken)
                          ?? $"Core service returned {(int)response.StatusCode} while deleting habit.";
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

    private sealed record CreateHabitResponseDto(Guid? HabitId);
}
