// Code inspired by https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;

namespace ChirpDatabase.Tests;
public class ChripDatabaseContextTest : IClassFixture<DatabaseFixture>
{
    private readonly ChirpDBContext context;

    public ChripDatabaseContextTest(DatabaseFixture _fixture)
    {
        context = _fixture.GetContext();
    }

    // creates a new user in the database with a username and an optional email, then checks if the user has been successfully saved
    [Theory]
    [InlineData("Cowboy", "WeeellHeeell@Yeehaw.com")]
    [InlineData("Jackson", null)]
    public async void CreateUser(string username, string? email)
    {
        // Act
        var user = new User()
        {
            Name = username,
            Email = email ?? null,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var DBuser = await context.Users.Where(_user => _user.Name == username).FirstOrDefaultAsync();
        Assert.NotNull(DBuser);
    }

    // checks if the system throws a database update exception when trying to create a user with an invalid email
    [Theory]
    [InlineData("MyEmailIsTooLong", "EEEEEEEEEEEEEEEEEEEEMMMMMMMMMMMMMMMMMMAAAAAAAAAAAIIIIIIILLLLLLL@TTTHHHIIISSSIIIISSSAAAAMMMMAAIIILLL.com")]
    public async void CreateUserWithInvalidEmail_ThrowsException(string name, string email)
    {
        // Act
        var user = new User()
        {
            Name = name,
            Email = email,
        };

        context.Users.Add(user);
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async() => await context.SaveChangesAsync());
        context.Users.Remove(user);
    }

    // verifies if the system throws a database update exception when attempting to create a second user with the same name and email as an existing user
    [Theory]
    [InlineData("Simon", "NormalMail@gmail.com")]
    public async void CreateUserWithSameName_ThrowsException(string name, string email)
    {
        // Act
        var normalUser = new User()
        {
            Name = name,
            Email = email,
        };
        var user = new User()
        {
            Name = name,
            Email = email,
        };

        context.Users.Add(normalUser);
        await context.SaveChangesAsync();
        
        context.Users.Add(user);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async() => await context.SaveChangesAsync());
    }

    // confirms the creation of an author associated with a user
    [Fact]
    public async void CreateAuthor()
    {
        // Act
        var user = await context.Users.Where(_user => _user.UserId == 2).FirstOrDefaultAsync(); // Get user from seeded data. UserId 1 is allready used
        var author = new Author()
        {
            User = user!,
            Cheeps = new List<Cheep>(),
        };

        context.Authors.Add(author);
        await context.SaveChangesAsync();

        // Assert
        var DBauthor = await context.Authors.Where(_author => _author.AuthorId == 1).FirstOrDefaultAsync();
        Assert.NotNull(DBauthor);
    }

    // tests the creation of a cheep by an author
    [Theory]
    [InlineData("New cheep here")]
    public async void CreateCheep(string message)
    {
        // Act
        var author = await context.Authors.Where(_author => _author.AuthorId == 1).FirstOrDefaultAsync(); // Author from CreateAuthor test

        var cheep = new Cheep()
        {
            Author = author!,
            Text = message,
            TimeStamp = DateTime.Now,
        };
        
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        // Assert
        var DBcheep = await context.Cheeps.Where(_cheep => _cheep.Text == "Cheep message").FirstOrDefaultAsync();
        Assert.NotNull(DBcheep);
    }

    // tests too long of a cheep, where the text content exceeds the allowed length of 160 characters
    [Fact]
    public async void CreateInvalidCheep_ThrowsException()
    {
        // Act
        var author = await context.Authors.Where(_author => _author.AuthorId == 1).FirstOrDefaultAsync(); // Author from CreateAuthor test

        var cheep = new Cheep()
        {
            Author = author!,
            Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
            TimeStamp = DateTime.Now,
        };

        context.Cheeps.Add(cheep);
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async() => await context.SaveChangesAsync());
        context.Remove(cheep);
    }

    // tests the creation of a follow, where one user follows another
    [Fact]
    public async void CreateFollow()
    {
        // Act
        var follow = new Follows()
        {
            FollowerId = 1,
            FollowingId = 2,
        };

        context.Follows.Add(follow);
        await context.SaveChangesAsync();

        // Assert
        var DBfollow = await context.Follows.Where(_follow => _follow.FollowerId == 1).FirstOrDefaultAsync();
        Assert.NotNull(DBfollow);
    }

    // tests a reaction, where a user upvotes a cheep
    [Fact]
    public async void CreateReaction()
    {
        // Act
        var cheep = await context.Cheeps.Where(_cheep => _cheep.CheepId == 1).FirstOrDefaultAsync();
        var user = await context.Users.Where(_user => _user.UserId == 1).FirstOrDefaultAsync();

        var reaction = new Reaction()
        {
            cheepId = cheep!.CheepId,
            userId = user!.UserId,
            reactionType = "Upvote",
        };
        
        context.Reactions.Add(reaction);
        await context.SaveChangesAsync();

        // Assert
        var DBreaction = await context.Reactions.Where(_reaction => _reaction.cheepId == cheep.CheepId && _reaction.userId == user.UserId).FirstOrDefaultAsync();
        Assert.NotNull(DBreaction);
    }
}