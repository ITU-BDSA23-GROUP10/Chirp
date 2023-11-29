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
    [InlineData("test2", "test2@test.de", "This is a test cheep2")]
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
                await cr.CreateCheep(cheep, await ar.GetAuthorByName(authorName));
            }
            catch 
            {
                await ur.CreateUser(authorName, authorEmail);
                await ar.CreateAuthor( await ur.GetUserByName(authorName) );
                await cr.CreateCheep(cheep, await ar.GetAuthorByName(authorName));
            }

            // Assert
            var retrievedAuthor = await ar.GetAuthorByName(authorName);

            Assert.Equal(authorName, retrievedAuthor.User.Name);
            Assert.Equal(message, retrievedAuthor.Cheeps[0].Text);
            }
    }
}