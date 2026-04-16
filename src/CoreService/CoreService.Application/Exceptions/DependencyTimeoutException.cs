namespace CoreService.Application.Exceptions;

public class DependencyTimeoutException : Exception
{
    public DependencyTimeoutException(string message) : base(message) { }
}
