using Core.Application.Abstractions.Persistence;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);

            services.AddScoped<IContextTransactionCreator, MocContextTransactionCreator>();

            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
            {
                options.UseSqlServer("Server=WIN-OOVMR6DQ7B5;database=TodosTests;Integrated Security=False;User Id=rr;Password=12345678;MultipleActiveResultSets=True;Trust Server Certificate=true;");
            });
        });

        builder.UseEnvironment("Development");
    }
}