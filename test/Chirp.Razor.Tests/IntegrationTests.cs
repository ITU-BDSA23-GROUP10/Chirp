namespace Chirp.Razor.Tests.Integration;
using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Razor.Tests.MemoryFactory;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure;
using Chirp.Infrastructure.ChirpRepository;

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
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/public");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("public's Timeline", content);
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

    // checks if unknown author has cheeps
    [Theory]
    [InlineData("Vobiscum")]
    [InlineData("Ad Astra")]
    public async void CheckIfAuthorHasNoCheeps(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("There are no cheeps so far.", content);
    }

    // is the root page the same as page 1?
    [Theory]
    [InlineData("/")]
    [InlineData(@"/?page=1")]
    public async void CheckIfTheRootPageTheSameAsPageOne(string page)
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
    public async void CheckIfThereThirtyTwoCheepsPerPage(string page)
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

            context.Authors.Add(new Author { Name = "nayhlalolk", Email = "oiw33e@gmail.com" });
            context.Authors.Add(new Author { Name = "testtesttesttesttest", Email = "testtest@hotmail.com" });
            await context.SaveChangesAsync();
        }

        //Assert
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            var author = context.Authors.FirstOrDefault(a => a.Name == "nayhlalolk" && a.Email == "oiw33e@gmail.com");
            var nonauthor = context.Authors.FirstOrDefault(a => a.Name == "check" && a.Email == "check");

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
            var author = context.Authors.FirstOrDefault(a => a.Name == "nayhlalolk" && a.Email == "oiw33e@gmail.com");

            Assert.Null(author);
        }
    }

    //Tests if the program can create a cheep using a try catch to ensure the author exists
    [Theory]
    [InlineData("test1", "test1@test.dk", "This is a test cheep1")]
    [InlineData("test2", "test2@test.de", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_CreateAuthorAfterException(string authorName, string authorEmail, string message) 
    {
        // This test shows how we should use the different methods to properly create cheeps
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);

            // Act            
            try 
            {
                cr.CreateCheep(ar.GetAuthorByName(authorName), message);
            } catch 
            {
                ar.CreateAuthor(authorName, authorEmail);
                await context.SaveChangesAsync();
                cr.CreateCheep(ar.GetAuthorByName(authorName), message);
            } finally 
            {
                await context.SaveChangesAsync();
            }

            // Assert
            var retrievedAuthor = ar.GetAuthorByName(authorName);
            Assert.Equal(authorName, retrievedAuthor.Name);
            Assert.Equal(message, retrievedAuthor.Cheeps[0].Text);
        }
    }
}