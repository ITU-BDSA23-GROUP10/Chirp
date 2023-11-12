using Testcontainers.MsSql;
using Bogus;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;

namespace ChirpIntegraiton.Tests;

public class ChirpDatabaseTest : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public Task InitializeAsync()
    {
        return _sqlServer.StartAsync();
    }
    public Task DisposeAsync()
    {
        return _sqlServer.DisposeAsync().AsTask();
    }

    [Fact]
    public async void CreateACheepInDatabase_WhereAuthorExists()
    {   
        // Assert
        var contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlServer(_sqlServer.GetConnectionString())
            .Options;
        var context = new ChirpDBContext(contextOptions);
        context.Database.Migrate();

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);

        // Act
        var authorName = "Test author";
        await authorService.CreateAuthor(authorName);
        var cheep = new CheepCreateDTO("Test message", authorName);

        await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        
        var dbCheep = cheepService.GetAll();

        // Assert
        Assert.Equal(1, dbCheep.Item2);
        Assert.Equal("Test message", dbCheep.Item1.FirstOrDefault().Text);
    }
}