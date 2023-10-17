namespace Chirp.Razor.Tests.Integration;
using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Razor.Tests.MemoryFactory;
using Microsoft.Extensions.DependencyInjection;
using SimpleDB;
using SimpleDB.Models;

public class IntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public IntegrationTest(CustomWebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, HandleCookies = true });
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
    [InlineData(@"/?page=0")]
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

    [Fact]
    public async Task InMemoryDatabase_ShouldNotPersistData()
    {
        //arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        //act
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            context.Authors.Add(new Author{Name = "nayhlalolk", Email = "oiwe"});
            context.Authors.Add(new Author{Name = "testtesttesttesttest", Email = "testtest@hotmail.com"});
            await context.SaveChangesAsync();
        }

        //Assert
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            var author = context.Authors.FirstOrDefault(a => a.Name == "nayhlalolk" && a.Email == "oiwe" );

            //this is used to check if the data comes into the in-memory database
            var nonauthor = context.Authors.FirstOrDefault(a => a.Name == "check" && a.Email == "check" );

            Assert.NotNull(author);
            Assert.Null(nonauthor); 
        }
    }

    [Fact]
    public async Task InMemoryDatabase_ShouldNotPersistData_test2()
    {
        //arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();
        
        //Assert
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            var author = context.Authors.FirstOrDefault(a => a.Name == "nayhlalolk" && a.Email == "oiwe" );
            
            Assert.Null(author);
        }
    }

    [Fact]
    public async Task CreateAuthorInDatabase_DoesntExist()
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            //Somehow use the repo methods to create a user here or outside the scope idk
        } 
    }

    [Fact]
    public async Task CreateAuthorInDatabase_DoesExist() 
    {
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            //Somehow use the repo methods to create a user here or outside the scope idk
        } 
    }

    [Fact]
    public async Task CreateCheepInDatabase_AuthorDoesntExist() 
    {
    }

    [Fact]
    public async Task CreateCheepInDatabase_AuthorDoesExist() 
    {
    }

}