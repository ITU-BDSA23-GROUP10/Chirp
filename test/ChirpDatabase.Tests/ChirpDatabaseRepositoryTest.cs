using Testcontainers.MsSql;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Chirp.Web;

namespace ChirpDatabase.Tests;

// TODO: Create order for test to awoid need for Seeding test database
public class ChirpDatabaseRepositoryTest : IClassFixture<DatabaseFixture>
{
    private readonly IUserRepository<User> userService;
    private readonly IAuthorRepository<Author, Cheep, User> authorService;
    private readonly ICheepRepository<Cheep, Author> cheepService;
    private readonly IFollowsRepository<Follows> followService;
    private readonly IReactionRepository<Reaction> reactionService;
    private readonly AsyncPadlock padlock;
    public ChirpDatabaseRepositoryTest(DatabaseFixture _fixture)
    {
        var context = _fixture.GetContext();
        userService = new UserRepository(context);
        authorService = new AuthorRepository(context);
        cheepService = new CheepRepository(context);
        followService = new FollowsRepository(context);
        reactionService = new ReactionRepository(context);

        padlock = _fixture.padlock;
    }

    // UserRepo tests
    [Theory]
    [InlineData("NewChirpUser", "user@usingChirp.com")]
    [InlineData("New user with spaces", "FancyUser@fancy.co.uk")]
    public async void CreateUserWithEmail(string name, string email)
    {
        // Act
        await userService.CreateUser(name, email);
        
        // Assert
        var user = await userService.GetUserByName(name);
        Assert.Equal(name, user!.Name);
        Assert.Equal(email, user!.Email);
    }

    [Theory]
    [InlineData("Anakin", "skywalker@jedi.com")]
    public async void DeleteUser(string name, string email)
    {
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
        var username = "Jill"; // Seeded user from DatabaseFixture
        // Assert
        Assert.Equal(1, await userService.GetUserIDByName(username));
    }

    [Fact]
    public async void UpdateUserEmail()
    {
        var username = "Jack"; // Seeded user from DatabaseFixture
        var email = "NewEmail@testmail.co.uk";
        // Act
        await userService.UpdateUserEmail(username, email);

        // Assert
        var user = userService.GetUserByEmail(username);
        Assert.NotNull(user);
    }

    [Fact]
    public async void GetUserById()
    {
        var username = "Jill"; // Seeded user with id = 1 in DatabaseFixture

        // Assert
        Assert.Equal(username, (await userService.GetUserById(1))!.Name);
    }

    [Fact]
    public async void CreateUser_NameExists()
    {
        var username = "Jill"; // Seeded user from DatabaseFixture

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await userService.CreateUser(username));
    }
    
    // Follows repo tests
    [Fact]
    public async void TwoUsersCanFollow_AndUnfollow()
    {
        // Follow
        var newFollow = new FollowDTO(1, 2); // Ids of seeded users from DatabaseFixture
        await followService.FollowUser(newFollow);

        // Assert Follow
        var follows = await followService.GetFollowedUsersId(1);
        Assert.NotEmpty(follows);
        Assert.Equal(2, follows[0]);
        Assert.True(await followService.IsFollowing(1, 2));

        // Unfollow
        await followService.UnfollowUser(newFollow);

        // Assert Unfollow
        follows = await followService.GetFollowedUsersId(1);
        Assert.Empty(follows);
        Assert.False(await followService.IsFollowing(1, 2));
    }

    // AuthorRepo test
    [Fact]
    public async void CreateAuthor()
    {
        var authorName = "Jack";
        
        // Act
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);

        // Assert
        var author = await authorService.GetAuthorByName(authorName);
        Assert.NotNull(author);
    }

    [Fact]
    public async void DeleteUserAlsoDeletesAuthor()
    {
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
        var username = "Jill"; // Seeded author from DatabaseFixture

        // Act
        var user = await userService.GetUserByName(username);

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await authorService.CreateAuthor(user!));
    }

    [Fact]
    public async void GetAuthorByName()
    {
        var username = "Jill";

        // Assert
        var author = await authorService.GetAuthorByName(username);
        Assert.NotNull(author);
        Assert.Equal(username, author.User.Name);
    }

    [Fact]
    public async void GetCheepsByAuthorOnlyGetsTheLimitOfCheeps() 
    {
        var username = "CheepAuthor";
        
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
        // Act
        await userService.CreateUser(authorName);
        var user = await userService.GetUserByName(authorName);
        await authorService.CreateAuthor(user!);
        var cheep = new CheepCreateDTO(message, authorName);

        var author = await authorService.GetAuthorByName(authorName);
        await cheepService.CreateCheep(cheep, author!);
        
        var dbCheep = await authorService.GetAllCheepsByAuthorName(authorName);

        // Assert
        Assert.Single(dbCheep);
        Assert.Equal(message, dbCheep[0].Message);
    }

    [Fact]
    public async void CreateValidCheep_WhereAuthorDoesntExist()
    {
        // Act
        var cheep = new CheepCreateDTO("Test", "I dont exist");

        // Assert
        await Assert.ThrowsAsync<Exception>(async() => await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(cheep.author)));
    }

    [Fact]
    public async void Create100CheepsWith100DifferentAuthors_ReadMostResent32()
    {
        // Act
        try {
            await padlock.Lock();
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
        }
        finally
        {
            padlock.Dispose();
        }

        // Assert
        var cheeps = await cheepService.GetSome(0, 32);
        Assert.Equal(32, cheeps.Item1.Count); // Only getting 32 cheeps
    }

    [Fact]
    public async void CreateInvalidCheep()
    {
        var authorName = "Jill";
        // Act
        var cheep = new CheepCreateDTO("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", authorName);

        // Assert
        var beforeCheeps = cheepService.GetAll();
        await Assert.ThrowsAsync<Exception>(async() => await cheepService.CreateCheep(cheep, (await authorService.GetAuthorByName(authorName))! ));      
        var afterCheeps = cheepService.GetAll();
        Assert.True(beforeCheeps.Item2 == afterCheeps.Item2);
    }
}