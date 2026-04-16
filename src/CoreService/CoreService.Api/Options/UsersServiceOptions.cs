namespace CoreService.Api.Options;

public class UsersServiceOptions
{
    public string BaseUrl { get; set; } = "";

    public HttpResilienceOptions Resilience { get; set; } = new();
}

public class HttpResilienceOptions
{
    public int MaxRetryAttempts { get; set; } = 2;

    public int AttemptTimeoutSeconds { get; set; } = 5;

    public int TotalRequestTimeoutSeconds { get; set; } = 20;
}
