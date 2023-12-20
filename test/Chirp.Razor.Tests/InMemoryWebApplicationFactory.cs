namespace Chirp.Razor.Tests.MemoryFactory;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure;

// referenced from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
// The CustomWebApplicationFactory class is a test utility class
// It configures a mock web host with an in-memory database for the ChirpDBContext
// This allows us to perform isolated testing without affecting a real database, and also to seed data for the testing environment.
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var cheepDBContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ChirpDBContext>));

            if (cheepDBContextDescriptor != null)
            {
                services.Remove(cheepDBContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<ChirpDBContext>(options =>
            {
                options.UseInMemoryDatabase("DataSource=:memory:");
            });

            using var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ChirpDBContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
        });

        builder.UseEnvironment("Development");
    }
}