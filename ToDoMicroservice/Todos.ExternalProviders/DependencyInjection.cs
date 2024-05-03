using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todos.Application.Abstractions.ExternalProviders;

namespace Todos.ExternalProviders;

public static class DependencyInjection
{
    public static IServiceCollection AddExternalProviders(this IServiceCollection services)
    {
        return services.AddTransient<IOwnersProvider, OwnersProvider>();
    }
}