using System.Net;
using Core.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Core.Api.Middlewares;

internal class CoreExceptionsHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;
        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = System.Text.Json.JsonSerializer.Serialize(validationException.Errors);
                break;
            case BadOperationException badOperationException:
                code = HttpStatusCode.BadRequest;
                result = System.Text.Json.JsonSerializer.Serialize(badOperationException.Message);
                break;
            case NotFoundException notFound:
                code = HttpStatusCode.NotFound;
                result = System.Text.Json.JsonSerializer.Serialize(notFound.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (result == string.Empty)
            result = System.Text.Json.JsonSerializer.Serialize(new {error = exception.Message, innerMessage = exception.InnerException?.Message, exception.StackTrace});

        return context.Response.WriteAsync(result);
    }
}

public static class CoreExceptionsHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCoreExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CoreExceptionsHandlerMiddleware>();
    }
}