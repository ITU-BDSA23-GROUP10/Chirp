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

    private async Task<ChirpDBContext> SetupContext(string ConnectionString)
    {
        var contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        var context = new ChirpDBContext(contextOptions);
        context.Database.Migrate();
        
        return context;
    }

    [Theory]
    [InlineData("Obi-Wan", "Hello there")]
    [InlineData("Grievous", "General Kenobi. A bold one i see")]
    public async void CreateValidCheepInDatabase_WhereAuthorExists(string authorName, string message)
    {   
        // Arrange
        var context = await SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);

        // Act
        await authorService.CreateAuthor(authorName);
        var cheep = new CheepCreateDTO(message, authorName);

        await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        
        var dbCheep = cheepService.GetAll();

        // Assert
        Assert.Equal(1, dbCheep.Item2);
        Assert.Equal(message, dbCheep.Item1.FirstOrDefault().Text);
    }

    [Fact]
    public async void Create100Cheeps_ReadLast32()
    {
        // Arrange
        var context = await SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);

        // Act
        for (int i = 0; i < 100; i++)
        {
            var authorName = "Test author " + i;
            var newAuthor = authorService.CreateAuthor(authorName);
            var cheep = new CheepCreateDTO("Test message for author " + i, authorName);
            await newAuthor;
            cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        }

        while (cheepService.GetAll().Item2 != 100) {
            Thread.Sleep(40);
        }

        // Assert
        var allCheeps = cheepService.GetAll();
        var cheeps = await cheepService.GetSome(0, 32);
        Assert.Equal(100, allCheeps.Item2);
        Assert.Equal(32, cheeps.Item1.Count);
        Assert.Equal("Test message for author 99", cheeps.Item1.FirstOrDefault().Message);
    }
}