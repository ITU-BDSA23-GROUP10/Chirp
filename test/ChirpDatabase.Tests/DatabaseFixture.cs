// Code from https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
using Testcontainers.MsSql;

namespace ChirpDatabase.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();


    public string ConnectionString => _sqlServer.GetConnectionString();
    public string ContainerId => $"{_sqlServer.Id}";

    public Task InitializeAsync() => _sqlServer.StartAsync();

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();
}