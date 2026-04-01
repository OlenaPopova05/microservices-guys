using CoreService.Application.Exceptions;
using CoreService.Application.Interfaces;

namespace CoreService.Infrastructure;

public class UsersServiceClient : IUsersServiceClient
{
    private readonly HttpClient _httpClient;

    public UsersServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/users/{userId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            if (response.IsSuccessStatusCode)
                return true;

            throw new ServiceUnavailableException("Users service is unavailable.");
        }
        catch (HttpRequestException)
        {
            throw new ServiceUnavailableException("Users service is unavailable.");
        }
        catch (TaskCanceledException)
        {
            throw new ServiceUnavailableException("Users service is unavailable.");
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
