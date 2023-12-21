// Code from https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
using Chirp.Infrastructure;
using Chirp.Infrastructure.Models;
using Chirp.Web;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ChirpDatabase.Tests;
// DatabaseFixture is a setup class for testing purposes.
// It uses a Docker-based SQL Server container to provide a database with a predictable state before each test.
// It also implements the IAsyncLifetime interface to manage the lifecycle of the SQL Server container.
public class DatabaseFixture : IAsyncLifetime
{
    private ChirpDBContext? Context;
    public readonly AsyncPadlock padlock = new();
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    
    private void SetUpContext()
    {
        var contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlServer(_sqlServer.GetConnectionString())
            .Options;
        Context = new ChirpDBContext(contextOptions);
        Context.Database.Migrate();
    }

    private static void SeedDB(ChirpDBContext context)
    {
        // Users
        var user1 = new User()
        {
            Name = "Jill",
            Email = "Jill@Hotmail.com",
        };
        var user2 = new User()
        {
            Name = "Jack",
            Email = "Jack@Yahoo.co.uk",
        };
        context.Users.AddRange(user1, user2);
        context.SaveChanges();

        // Authors
        var author = new Author()
        {
            User = user1,
            Cheeps = new List<Cheep>(),
        };
        context.Authors.Add(author);
        context.SaveChanges();

        // Cheeps
        var cheep = new Cheep()
        {
            Author = author,
            Text = "Cheep message",
            TimeStamp = DateTime.Now,
        };
        context.Cheeps.Add(cheep);
        context.SaveChanges();
    }

    public Task InitializeAsync() => _sqlServer.StartAsync();

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    public ChirpDBContext GetContext()
    {
        if (Context is null)
        {
            SetUpContext();
            SeedDB(Context!);
            return Context!;
        }
        else
        {
            return Context!;
        }
    }
}