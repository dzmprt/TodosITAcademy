using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todos.Application.Abstractions.Persistence;
using Todos.Application.Abstractions.Persistence.Repository.Read;
using Todos.Application.Abstractions.Persistence.Repository.Writing;
using Todos.Persistence.Repositories;

namespace Todos.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddDbContext<DbContext, ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            })
            .AddScoped<IContextTransactionCreator, ContextTransactionCreator>()
            .AddTransient(typeof(IBaseReadRepository<>), typeof(BaseRepository<>))
            .AddTransient(typeof(IBaseWriteRepository<>), typeof(BaseRepository<>))
            .AddScoped<IDatabaseMigrator, DatabaseMigrator>();
    }
}