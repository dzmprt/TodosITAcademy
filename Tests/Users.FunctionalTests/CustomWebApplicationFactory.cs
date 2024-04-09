using System.Data.Common;
using Core.Application.Abstractions.Persistence;
using Core.Tests;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Users.FunctionalTests;

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
                options.UseSqlServer(
                   // "Server=.;database=TodosTests;Integrated Security=False;User Id=sa;Password=0bd7903b-f568-4894-8d72-3c1b507e5644;MultipleActiveResultSets=True;Trust Server Certificate=true;");
                   "Server=.;database=TodosTest;Integrated Security=False;User Id=db_den1;Password=den;MultipleActiveResultSets=True;Trust Server Certificate=true;");
            });
        });
        
        builder.UseEnvironment("Development");
    }
}