using Testcontainers.MsSql;
using Bogus;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace ChirpIntegraiton.Tests;

public class ChirpDatabaseRepositoryTest : IAsyncLifetime
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


    // UserRepo tests
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
        Assert.Equal(name, userByName!.Name);
        var userByEmail = await userService.GetUserByEmail(email);
        Assert.Equal(email, userByEmail!.Email);
    }

    [Theory]
    [InlineData("Anakin", "skywalker@jedi.com")]
    public async void DeleteUser(string name, string email)
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        // Act
        await userService.CreateUser(name, email);
        var userToBeDeleted = await userService.GetUserByName(name);
        await userService.DeleteUser(userToBeDeleted!);

        // Assert
        var userByName = await userService.GetUserByName(name);
        Assert.Null(userByName);
        var userByEmail = await userService.GetUserByEmail(email);
        Assert.Null(userByEmail);
    }

    [Fact]
    public async void GetIdOfUserByName()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        var username = "TestUser";
       
        // Act
        await userService.CreateUser(username);

        // Assert
        Assert.Equal(1, await userService.GetUserIDByName(username));
    }

    [Fact]
    public async void UpdateUserEmail()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        var username = "TestUser";
        var email = "TestUser@testmail.co.uk";
        // Act
        await userService.CreateUser(username);
        await userService.UpdateUserEmail(username, email);

        // Assert
        var user = userService.GetUserByEmail(username);
        Assert.NotNull(user);
    }

    [Fact]
    public async void GetUserById()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        var username = "TestUser";

        // Act
        await userService.CreateUser(username);

        // Assert
        Assert.Equal(username, (await userService.GetUserById(1))!.Name);
    }

    [Fact]
    public async void CreateUser_NameExists()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        var username = "TestUser";
        
        // Act
        await userService.CreateUser(username);

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await userService.CreateUser(username));
    }
    

    // Follows repo tests
    [Fact]
    public async void TwoUsersCanFollowEachOther()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);
        var followService = new FollowsRepository(context);

        var username1 = "Testuser1";
        var username2 = "Testuser2";

        // Act
        await userService.CreateUser(username1);
        await userService.CreateUser(username2);
        
        var newFollow = new FollowDTO(1, 2);
        await followService.FollowUser(newFollow);

        // Assert
        var follows = await followService.GetFollowedUsersId(1);
        Assert.NotEmpty(follows);
        Assert.Equal(2, follows[0]);
        Assert.True(await followService.IsFollowing(1, 2));
    }

    [Fact]
    public async void UsersCanUnfollow()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);
        var followService = new FollowsRepository(context);

        var username1 = "Testuser1";
        var username2 = "Testuser2";

        // Act
        await userService.CreateUser(username1);
        await userService.CreateUser(username2);

        var newFollow = new FollowDTO(1, 2);
        await followService.FollowUser(newFollow);

        await followService.UnfollowUser(newFollow);

        // Assert
        var follows = await followService.GetFollowedUsersId(1);
        Assert.Empty(follows);
        Assert.True(!await followService.IsFollowing(1, 2));
    }


    // AuthorRepo test
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
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);

        // Assert
        var author = await authorService.GetAuthorByName(authorName);
        Assert.NotNull(author);
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
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);

        // Act
        await userService.DeleteUser(user!);

        // Assert
        Assert.Null( await userService.GetUserByName(authorName) );
        Assert.Null( await authorService.GetAuthorByName(authorName) );
    }

    [Fact]
    public async void CreateAuthor_AlreadyExists()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);
        
        var username = "Obi-wan";
        // Act
        await userService.CreateUser(username);
        var user = await userService.GetUserByName(username);
        await authorService.CreateAuthor(user!);

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await authorService.CreateAuthor(user!));
    }

    [Fact]
    public async void GetAuthorByName()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);

        var username = "Testuser";

        // Act
        await userService.CreateUser(username);
        var user = await userService.GetUserByName(username);
        await authorService.CreateAuthor(user!);

        // Assert
        var author = await authorService.GetAuthorByName(username);
        Assert.NotNull(author);
        Assert.Equal(username, author.User.Name);
    }

    [Fact]
    public async void GetCheepsByAuthorOnlyGetsTheLimitOfCheeps() 
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);
        var cheepService = new CheepRepository(context);
        
        var username = "Testuser";
        // Act
        await userService.CreateUser(username);
        var user = await userService.GetUserByName(username);
        await authorService.CreateAuthor(user!);
        var author = await authorService.GetAuthorByName(username);

        for (int i = 0; i < 100; i++)
        {
            var newCheep = new CheepCreateDTO("Cheep" + 1, username);
            await cheepService.CreateCheep(newCheep, author!);
        }

        // Assert
        var cheeps = await authorService.GetCheepsByAuthor(username, 1, 32);
        Assert.Equal(100, cheeps.Item2);
        Assert.Equal(32, cheeps.Item1.Count);
    }


    // CheepRepo tests
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
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);
        var cheep = new CheepCreateDTO(message, authorName);

        var author = await authorService.GetAuthorByName(authorName);
        await cheepService.CreateCheep(cheep, author!);
        
        var dbCheep = cheepService.GetAll();

        // Assert
        Assert.Equal(1, dbCheep.Item2);
        Assert.Equal(message, dbCheep.Item1.FirstOrDefault()!.Text);
    }

    [Fact]
    public async void CreateValidCheep_WhereAuthorDoesntExist()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);

        // Act
        var cheep = new CheepCreateDTO("Test", "TestAuthor");

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(cheep.author)));
    }

    [Fact]
    public async void Create100CheepsWith100DifferentAuthors_ReadMostResent32()
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

            var user = await userService.GetUserByName(authorName);
            await authorService.CreateAuthor(user!);

            var cheep = new CheepCreateDTO("Test message for author " + i, authorName);
            var author =  await authorService.GetAuthorByName(authorName);
            
            await cheepService.CreateCheep(cheep, author!);
        }

        // Assert
        var allCheeps = cheepService.GetAll();
        var cheeps = await cheepService.GetSome(0, 32);
        Assert.Equal(100, allCheeps.Item2); // All cheeps are created
        Assert.Equal(32, cheeps.Item1.Count); // Only getting 32 cheeps
        Assert.Equal("Test message for author 99", cheeps.Item1.FirstOrDefault()!.Message); // The cheeps gotten is the most resent 
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
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);
        var cheep = new CheepCreateDTO("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", authorName);

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await cheepService.CreateCheep(cheep, (await authorService.GetAuthorByName(authorName))! ));      
        var cheeps = cheepService.GetAll();
        Assert.Equal(0, cheeps.Item2);
    }


    // ReactionRepo tests

}