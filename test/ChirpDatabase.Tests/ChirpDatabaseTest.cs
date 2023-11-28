using Testcontainers.MsSql;
using Bogus;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.Diagnostics.Contracts;

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

    private static ChirpDBContext SetupContext(string ConnectionString)
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
        var context = SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);

        // Act
        await userService.CreateUser(authorName);
        await authorService.CreateAuthor( await userService.GetUserByName(authorName) );
        var cheep = new CheepCreateDTO(message, authorName);

        await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        
        var dbCheep = cheepService.GetAll();

        // Assert
        Assert.Equal(1, dbCheep.Item2);
        Assert.Equal(message, dbCheep.Item1.FirstOrDefault().Text);
    }

    [Fact]
    public async void Create100Cheeps_ReadMostResent32()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);

        // Act
        for (int i = 0; i < 100; i++)
        {
            var authorName = "Test author " + i;
            await userService.CreateUser(authorName);
            await authorService.CreateAuthor( await userService.GetUserByName(authorName) );
            var cheep = new CheepCreateDTO("Test message for author " + i, authorName);
            await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        }

        // Assert
        var allCheeps = cheepService.GetAll();
        var cheeps = await cheepService.GetSome(0, 32);
        Assert.Equal(100, allCheeps.Item2); // All cheeps are created
        Assert.Equal(32, cheeps.Item1.Count); // Only getting 32 cheeps
        Assert.Equal("Test message for author 99", cheeps.Item1.FirstOrDefault().Message); // The cheeps gotten is the most resent 
    }

    [Theory]
    [InlineData("Obi-Wan", "obi-wan@jedi.com")]
    [InlineData("General Grievous", "xXjediSlayerXx@sith.co.uk")]
    public async void CreateUserWithEmail(string name, string email)
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        // Act
        await userService.CreateUser(name, email);
        
        // Assert
        var userByName = await userService.GetUserByName(name);
        Assert.Equal(name, userByName.Name);
        var userByEmail = await userService.GetUserByEmail(email);
        Assert.Equal(email, userByEmail.Email);
    }

    [Fact]
    public async void CreateInvalidCheep()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var userService = new UserRepository(context);
        var authorService = new AuthorRepository(context);
        var cheepService = new CheepRepository(context);

        var authorName = "Obi-Wan";
        // Act
        await userService.CreateUser(authorName);
        await authorService.CreateAuthor( await userService.GetUserByName(authorName) );
        var cheep = new CheepCreateDTO("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", authorName);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async() => await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName) ));      
        var cheeps = cheepService.GetAll();
        Assert.Equal(0, cheeps.Item2);
    }

    [Fact]
    public async void CreateAuthor()
    {
        // Arrage
        var context = SetupContext(_sqlServer.GetConnectionString());

        var userService = new UserRepository(context);
        var authorService = new AuthorRepository(context);

        var authorName = "Obi-Wan";
        
        // Act
        await userService.CreateUser(authorName);
        await authorService.CreateAuthor( await userService.GetUserByName(authorName) );

        // Assert
        var author = await authorService.GetAuthorByName(authorName);
        Assert.NotNull(author);
    }

    [Fact]
    public async void DeleteUser()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var userService = new UserRepository(context);

        await userService.CreateUser("Test1");
        await userService.CreateUser("Test2");
        
        // Act
        userService.DeleteUser( await userService.GetUserByName("Test1") );

        // Assert
        Assert.Null( await userService.GetUserByName("Test1") );
        Assert.NotNull( await userService.GetUserByName("Test2"));
    }

    [Fact]
    public async void DeleteUserAlsoDeletesAuthor()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var userService = new UserRepository(context);
        var authorService = new AuthorRepository(context);

        var authorName = "Test1";
        await userService.CreateUser(authorName);
        await authorService.CreateAuthor( await userService.GetUserByName(authorName) );

        // Act
        userService.DeleteUser( await userService.GetUserByName(authorName) );

        // Assert
        Assert.Null( await userService.GetUserByName(authorName) );
        Assert.Null( await authorService.GetAuthorByName(authorName) );
    }
}