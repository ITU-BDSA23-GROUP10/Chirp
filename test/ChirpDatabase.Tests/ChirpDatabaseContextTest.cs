using Testcontainers.MsSql;
using Bogus;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace ChirpDatabase.Tests;

public class ChripDatabaseContextTest : IAsyncLifetime
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


    // Users tests
    [Theory]
    [InlineData("Cowboy", "WeeellHeeell@Yeehaw.com")]
    [InlineData("Jackson", null)]
    public async void CreateUser(string username, string? email)
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

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

    [Theory]
    [InlineData("Jackson", "EEEEEEEEEEEEEEEEEEEEMMMMMMMMMMMMMMMMMMAAAAAAAAAAAIIIIIIILLLLLLL@TTTHHHIIISSSIIIISSSAAAAMMMMAAIIILLL.com")]
    [InlineData("Simon", "NormalMail@gmail.com")]
    public async void CreateInvalidUser_ThrowsException(string name, string email)
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        // Act
        var normalUser = new User()
        {
            Name = "Simon",
            Email = "Email@gmail.com",
        };
        context.Users.Add(normalUser);
        await context.SaveChangesAsync();

        var user = new User()
        {
            Name = name,
            Email = email,
        };
        context.Users.Add(user);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async() => await context.SaveChangesAsync());
    }


    // Author tests
    [Fact]
    public async void CreateAuthor()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var username = "Jackson";
        
        // Act
        var user = new User()
        {
            Name = username,
            Email = null,
        };
        context.Users.Add(user);

        var author = new Author()
        {
            User = user,
            Cheeps = new List<Cheep>(),
        };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        // Assert
        var DBauthor = await context.Authors.Where(_author => _author.User.Name == username).FirstOrDefaultAsync();
        Assert.NotNull(DBauthor);
    }


    // Cheep tests
    [Fact]
    public async void CreateCheep()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var username = "Jackson";

        // Act
        var user = new User()
        {
            Name = username,
            Email = null,
        };
        context.Users.Add(user);

        var author = new Author()
        {
            User = user,
            Cheeps = new List<Cheep>(),
        };
        context.Authors.Add(author);

        var cheep = new Cheep()
        {
            Author = author,
            Text = "Cheep message",
            TimeStamp = DateTime.Now,
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        // Assert
        var DBcheep = await context.Cheeps.Where(_cheep => _cheep.Text == "Cheep message").FirstOrDefaultAsync();
        Assert.NotNull(DBcheep);
    }

    [Fact]
    public async void CreateInvalidCheep_ThrowsException()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var username = "Jackson";

        // Act
        var user = new User()
        {
            Name = username,
            Email = null,
        };
        context.Users.Add(user);

        var author = new Author()
        {
            User = user,
            Cheeps = new List<Cheep>(),
        };
        context.Authors.Add(author);

        var cheep = new Cheep()
        {
            Author = author,
            Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries,",
            TimeStamp = DateTime.Now,
        };
        context.Cheeps.Add(cheep);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async() => await context.SaveChangesAsync());
    }


    // 
}