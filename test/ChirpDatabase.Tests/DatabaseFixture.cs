// Code from https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ChirpDatabase.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    private ChirpDBContext? Context;
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

    public Task InitializeAsync() => _sqlServer.StartAsync();

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    public ChirpDBContext GetContext()
    {
        if (Context is null)
        {
            SetUpContext();
            return Context!;
        }
        else
        {
            return Context!;
        }
    }
}