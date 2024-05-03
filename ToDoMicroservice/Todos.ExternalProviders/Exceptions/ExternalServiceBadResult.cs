namespace Todos.ExternalProviders.Exceptions;

public class ExternalServiceBadResult: Exception
{
    public ExternalServiceBadResult(string serviceName, string message) : base($"serviceName: {serviceName}, message: {message}")
    {
        
    }
}