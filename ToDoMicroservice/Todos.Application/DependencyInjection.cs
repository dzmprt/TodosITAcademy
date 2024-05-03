using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todos.Application.Behavior;
using Todos.Application.Caches;

namespace Todos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTodosApplicationsServices(this IServiceCollection services)
    {
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true)
            .AddSingleton<TodoMemoryCache>()
            .AddSingleton<TodosListMemoryCache>()
            .AddSingleton<TodosCountMemoryCache>()
            .AddTransient<ICleanTodosCacheService, CleanTodosCacheService>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(DatabaseTransactionBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizePermissionsBehavior<,>));
    }
}