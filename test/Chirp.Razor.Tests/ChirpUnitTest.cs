using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Razor.Tests.MemoryFactory;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Infrastructure.Models;

[Collection("Sequential")]
public class ChirpUnitTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public ChirpUnitTests(CustomWebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    //Tests if the publictimline is visable and contains the elements for it to be correct
    [Fact]
    public async Task CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/public");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("public's Timeline", content);
    }

    //Tests if the program can create an author when it doesn't exist in the database
    [Theory]
    [InlineData("test1", "test1@test.dk")]
    [InlineData("test2", "test2@test.de")]
    public async Task CreateAuthorInDatabase_DoesntExist(string authorName, string authorEmail)
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);

            // Act
            ar.CreateAuthor(authorName, authorEmail);
            await context.SaveChangesAsync();

            // Assert
            Author? retrievedAuthor = ar.GetAuthorByName(authorName);
            
            if (retrievedAuthor is null) 
            {
                Assert.Fail("Retrieved Author was null.");
            }
            else 
            {
                Assert.Equal(authorName, retrievedAuthor.Name);
                Assert.Equal(authorEmail, retrievedAuthor.Email);
            }
        }
    }

    //Tests if the program throws an exception when trying to create an author that already exists in the database
    [Theory]
    [InlineData("test1", "test1@test.dk")]
    [InlineData("test2", "test2@test.de")]
    public async Task CreateAuthorInDatabase_DoesExist(string authorName, string authorEmail)
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);

            // Act
            ar.CreateAuthor(authorName, authorEmail);
            await context.SaveChangesAsync();

            // Assert
            Assert.Throws<Exception>(() => ar.CreateAuthor(authorName, authorEmail));
            //Assert.Equal(authorName, retrievedAuthor.Name);
        }
    }

    // Tests to see if the program can create a cheep for an author that exists
    [Theory]
    [InlineData("test1", "test1@test.dk", "This is a test cheep1")]
    [InlineData("test2", "test2@test.de", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_AuthorExists(string authorName, string authorEmail, string message) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();


        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);

            // Act
            ar.CreateAuthor(authorName, authorEmail);
            await context.SaveChangesAsync();

            cr.CreateCheep(ar.GetAuthorByName(authorName), message);
            await context.SaveChangesAsync();

            // Assert
            var retrievedAuthor = ar.GetAuthorByName(authorName);

            if (retrievedAuthor is null) 
            {
                Assert.Fail("Retrieved Author was null.");
            }
            else 
            {
                Assert.Equal(authorName, retrievedAuthor.Name);
                Assert.Equal(message, retrievedAuthor.Cheeps[0].Text);
            }
        }
    }

    //Tests if the program throws an exception when the program tries to create a cheep for an author that doesn't exist
    [Theory]
    [InlineData("test1", "This is a test cheep1")]
    [InlineData("test2", "This is a test cheep2")]
    public async Task CreateCheep_ThrowsException_WhenAuthorDoesNotExist(string authorName, string message) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);
            await context.SaveChangesAsync();
            // Assert
            Assert.Throws<Exception>(() => cr.CreateCheep(ar.GetAuthorByName(authorName), message));
        }
    }

    // Tests if the program can duplicate an author object to another author obj from the database
    [Fact]
    public void DuplicateAuthorObjInDatabase_ListOfCheepsIsNotEmpty()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new(context);

            // Act
            var author = context.Authors.Include(a => a.Cheeps).FirstOrDefault(a => a.AuthorId == 10);

            // Assert
            Assert.NotNull(author);
            Assert.Equal("Jacqualine Gilcoine", author.Name);
            Assert.Equal("Jacqualine.Gilcoine@gmail.com", author.Email);
            Assert.NotEmpty(author.Cheeps);
        }
    }
}