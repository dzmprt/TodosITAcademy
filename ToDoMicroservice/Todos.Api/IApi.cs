namespace Todos.Api;

public interface IApi
{
    void Register(WebApplication app, string baseApiUrl = "");
}