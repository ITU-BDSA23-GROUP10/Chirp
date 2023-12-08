namespace Chirp.Razor.Tests.Integration;
using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Razor.Tests.MemoryFactory;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Chirp.Web.ViewComponents;

[Collection("Sequential")]
//referenced from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
public class IntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public IntegrationTest(CustomWebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    // checks if the timeline has content
    [Fact]
    public async Task CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    // checks if authors (from dump.sql) have content
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Roger Histand")]
    [InlineData("Luanna Muro")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test3")]
    public async Task UserDoesntExistPageErrorTest(string authorName) 
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var response = await _client.GetAsync("/" + authorName);
            response.EnsureSuccessStatusCode();

            var contentPage = await response.Content.ReadAsStringAsync();

            string UserDoesntExist = "User " + authorName + " does not exist";
            string GoBackHome = "Go back to the home page";

            Assert.Contains(UserDoesntExist, contentPage);
            Assert.Contains(GoBackHome, contentPage);
        } 
    }

    // Checks if author, with no cheeps, has no cheeps.
    [Theory]
    [InlineData("Vobiscum")]
    [InlineData("Ad Astra")]
    public async Task CheckIfAuthorHasNoCheeps(string author)
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        //act
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            var newUser = new User { Name = author, Email = $"test@gmail.com" };
            context.Users.Add(newUser);
            context.Authors.Add(new Author { User = newUser });
            await context.SaveChangesAsync();
            
            var response = await _client.GetAsync($"/{author}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("There are no cheeps so far.", content);
        } 
    }

    // is the root page the same as page 1?
    [Theory]
    [InlineData("/")]
    [InlineData(@"/?page=1")]
    public async Task CheckIfTheRootPageTheSameAsPageOne(string page)
    {
        var response = await _client.GetAsync(page);
        response.EnsureSuccessStatusCode();

        var contentPage = await response.Content.ReadAsStringAsync();

        string firstCheepAuthor = "<a href=\"/Jacqualine Gilcoine\">Jacqualine Gilcoine</a>";
        string firstCheepMessage = "Starbuck now is what we hear the worst.";

        Assert.Contains("<h2> Public Timeline </h2>", contentPage);
        Assert.Contains(firstCheepAuthor, contentPage);
        Assert.Contains(firstCheepMessage, contentPage);
    }

    // checks if there are 32 cheeps per page (this test uses Anglesharp)
    // stitched together from https://github.com/AngleSharp/AngleSharp/blob/devel/docs/general/01-Basics.md
    [Theory]
    [InlineData("?page=1")]
    [InlineData("?page=2")]
    [InlineData("?page=3")]
    public async Task CheckIfThereThirtyTwoCheepsPerPage(string page)
    {
        var response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(content));
        var listItems = document.QuerySelectorAll("ul#messagelist li");

        Assert.Equal(32, listItems.Length);
    }

    //checks the functionality of adding new authors to a database (in memory only)
    [Fact]
    public async Task AddingAuthors_WithContext_AddsToDatabase()
    {
        //arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        //act
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            var newUser1 = new User(){ Name = "nayhlalolk", Email = "oiw33e@gmail.com" };
            var newUser2 = new User(){ Name = "testtesttesttesttest", Email = "testtest@hotmail.com" };
            context.Users.Add(newUser1);
            context.Users.Add(newUser2);
            context.Authors.Add(new Author { User = newUser1 });
            context.Authors.Add(new Author { User = newUser2 });
            await context.SaveChangesAsync();
        }

        //Assert
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            var author = context.Authors.FirstOrDefault(a => a.User.Name == "nayhlalolk" && a.User.Email == "oiw33e@gmail.com");
            var nonauthor = context.Authors.FirstOrDefault(a => a.User.Name == "check" && a.User.Email == "check");

            Assert.NotNull(author);
            Assert.Null(nonauthor);
        }
    }

    // verifies that the database does not retain any data with the specific author in question after the test execution (in memory only).
    [Fact]
    public void VerifyingAuthors_WithContext_DoesNotRetainData()
    {
        //arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        //Assert
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            var author = context.Authors.FirstOrDefault(a => a.User.Name == "nayhlalolk" && a.User.Email == "oiw33e@gmail.com");

            Assert.Null(author);
        }
    }

    //Tests if the program can create a cheep using a try catch to ensure the author exists
    [Theory]
    [InlineData("test1", "test1@test.dk", "This is a test cheep1")]
    [InlineData("test3", "test3@test.de", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_CreateAuthorAfterException(string authorName, string authorEmail, string message) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            UserRepository ur = new UserRepository(context);

            var cheep = new CheepCreateDTO(message, authorName);
            // Act            
            try 
            {
                var author = await ar.GetAuthorByName(authorName);
                await cr.CreateCheep(cheep, author!);
            }
            catch 
            {
                await ur.CreateUser(authorName, authorEmail);
                var user = await ur.GetUserByName(authorName);
                await ar.CreateAuthor(user!);
                var author = await ar.GetAuthorByName(authorName);
                await cr.CreateCheep(cheep, author!);
            }

            // Assert
            var retrievedAuthor = await ar.GetAuthorByName(authorName);

            Assert.Equal(authorName, retrievedAuthor!.User.Name);
            Assert.Equal(message, retrievedAuthor.Cheeps[0].Text);
        }
    }

    //Tests if the program adds follows correctly and creates a user if the user does not exist
    [Theory]
    [InlineData("test1", "test1@test.dk", "test2", "test2@test.com")]
    [InlineData("test3", "test3@test.de", "test4", "test4@test.uk")] 
    public async Task CreateFollowInDatabase_CreateUserAfterException(string authorName, string authorEmail, string followName, string followEmail) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            UserRepository ur = new UserRepository(context);
            FollowsRepository fr = new FollowsRepository(context);
            // Creates the user for the follow if it does not exist
            try
            {
                var creatingAuthor = await ur.GetUserByName(followName);
                await ur.CreateUser(followName, followEmail);
                if(creatingAuthor != null)
                {
                    await ar.CreateAuthor(creatingAuthor);
                }  
            }
            catch
            {
                Assert.Fail("Failed to create follow_User");
            }
            // Act            
            try 
            {
                var userId = await ur.GetUserIDByName(authorName);
                if(userId == -1)
                {
                    await ur.CreateUser(authorName, authorEmail);
                    userId = await ur.GetUserIDByName(authorName);
                }
                var followID = await ur.GetUserIDByName(followName);

                var follow = new FollowDTO(userId, followID);
                await fr.FollowUser(follow);
            }
            catch 
            {
                Assert.Fail("Failed to create follow");
            }

            // Assert
            var retrievedUser = await ur.GetUserByName(authorName);
            var retrievedFollow = await ur.GetUserByName(followName);

            var UserFollows = await fr.IsFollowing(retrievedUser!.UserId, retrievedFollow!.UserId);
            var FollowDoesntFollowUser = await fr.IsFollowing(retrievedFollow.UserId, retrievedUser.UserId);

            Assert.Equal(authorName, retrievedUser.Name);
            Assert.Equal(followName, retrievedFollow.Name);
            Assert.True(UserFollows);
            Assert.False(FollowDoesntFollowUser);
        }     
    }

    [Theory]
    [InlineData("Upvote")]
    [InlineData("Downvote")] 
    public async Task CheckIfReactedByUser(string typeOfReaction) 
    {
        string authorName = "nametesttestname";
        string authorEmail = "test@test.com";
        string message = "Test";

        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var cheep = new CheepCreateDTO(message, authorName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            UserRepository ur = new UserRepository(context);
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            ReactionRepository rr = new ReactionRepository(context);

            // the user with the cheep
            await ur.CreateUser(authorName, authorEmail);
            var user = await ur.GetUserByName(authorName);
            await ar.CreateAuthor(user!);
            var author = await ar.GetAuthorByName(authorName);
            await cr.CreateCheep(cheep, author!);
            var cheepUser = cr.SearchFor(_cheepUser => _cheepUser.AuthorId == user!.UserId).FirstOrDefault();

            // the user that will like the cheep
            await ur.CreateUser("CheepLiker", "cheepliker69@gmail.com");
            var reactUser = await ur.GetUserByName("CheepLiker");
            ReactionDTO rd = new ReactionDTO(cheepUser!.CheepId, reactUser!.UserId, typeOfReaction);
            await rr.ReactToCheep(rd);
            var reaction = rr.SearchFor(_react => _react.userId == reactUser.UserId && _react.cheepId == cheepUser.CheepId).FirstOrDefault();
            
            Assert.True(reaction!.userId == reactUser.UserId);
        }
    }

    [Theory]
    [InlineData("Upvote", "Downvote")]
    [InlineData("Downvote", "Upvote")] 
    public async Task CheckIfReactedByUser_ChangedFromOneReactionToAnother(string firstReaction, string secoundReaction) 
    {
        string authorName = "nametesttestname";
        string authorEmail = "test@test.com";
        string message = "Test";

        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var cheep = new CheepCreateDTO(message, authorName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            UserRepository ur = new UserRepository(context);
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            ReactionRepository rr = new ReactionRepository(context);

            // the user with the cheep
            await ur.CreateUser(authorName, authorEmail);
            var user = await ur.GetUserByName(authorName);
            await ar.CreateAuthor(user!);
            var author = await ar.GetAuthorByName(authorName);
            await cr.CreateCheep(cheep, author!);
            var cheepUser = cr.SearchFor(_cheepUser => _cheepUser.AuthorId == user!.UserId).FirstOrDefault();

            // the user that will like the cheep
            await ur.CreateUser("CheepLiker", "cheepliker69@gmail.com");
            var reactUser = await ur.GetUserByName("CheepLiker");

            // Creating upvote reaction
            ReactionDTO rd = new ReactionDTO(cheepUser!.CheepId, reactUser!.UserId, firstReaction);
            await rr.ReactToCheep(rd);
            var reaction = rr.SearchFor(_react => _react.userId == reactUser.UserId && _react.cheepId == cheepUser.CheepId).FirstOrDefault();
            
            Assert.True(reaction!.userId == reactUser.UserId && reaction.reactionType == rd.reactionType);

            // creating downvote reaction
            rd = new ReactionDTO(cheepUser.CheepId, reactUser.UserId, secoundReaction);
            await rr.ReactToCheep(rd);
            reaction = rr.SearchFor(_react => _react.userId == reactUser.UserId && _react.cheepId == cheepUser.CheepId).FirstOrDefault();
            Assert.True(reaction!.userId == reactUser.UserId && reaction.reactionType == rd.reactionType);
        }
    }

    [Theory]
    [InlineData("Upvote")]
    [InlineData("Downvote")] 
    public async Task CheckIfReactedByUser_ChangedFromUpvoteOrDownvoteToNoReaction(string reactionToCheep) 
    {
        string authorName = "nametesttestname";
        string authorEmail = "test@test.com";
        string message = "Test";

        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var cheep = new CheepCreateDTO(message, authorName);
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            UserRepository ur = new UserRepository(context);
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            ReactionRepository rr = new ReactionRepository(context);

            // the user with the cheep
            await ur.CreateUser(authorName, authorEmail);
            var user = await ur.GetUserByName(authorName);
            await ar.CreateAuthor(user!);
            var author = await ar.GetAuthorByName(authorName);
            await cr.CreateCheep(cheep, author!);

            var cheepUser = cr.SearchFor(_cheepUser => _cheepUser.AuthorId == user!.UserId).FirstOrDefault();

            // the user that will like the cheep
            await ur.CreateUser("CheepLiker", "cheepliker69@gmail.com");
            var reactUser = await ur.GetUserByName("CheepLiker");

            // Creating upvote reaction
            ReactionDTO rd = new ReactionDTO(cheepUser!.CheepId, reactUser!.UserId, reactionToCheep);

            await rr.ReactToCheep(rd);
            var reaction = rr.SearchFor(_react => _react.userId == reactUser!.UserId && _react.cheepId == cheepUser!.CheepId).FirstOrDefault();
            //throw new Exception("test test test: " + rr.SearchFor(_react => _react.userId == reactUser.UserId && _react.cheepId == cheepUser.CheepId).Count());
            
            Assert.True(reaction!.userId == reactUser!.UserId && reaction.reactionType == rd.reactionType);

            // creating upvote reaction on the same cheep
            rd = new ReactionDTO(cheepUser!.CheepId, reactUser.UserId, reactionToCheep);
            await rr.ReactToCheep(rd);

            var ShouldbeNothing = rr.SearchFor(_react => _react.userId == reactUser.UserId && _react.cheepId == cheepUser.CheepId);
            
            bool reactionfound = false;
            if(ShouldbeNothing.Count() != 0)
            {
                reactionfound = true;
            }
            Assert.True(reactionfound == false);
        }
    }
}