namespace Todos.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Unauthorized")
    {
    }
}