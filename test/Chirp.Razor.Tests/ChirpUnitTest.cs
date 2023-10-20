using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Razor.Tests.MemoryFactory;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure;
using Chirp.Infrastructure.ChirpRepository;

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

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/public");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("public's Timeline", content);
    }


    [Theory]
    [InlineData("test1", "test1@test.dk")]
    [InlineData("test2", "test2@test.de")]
    public async Task CreateAuthorInDatabase_DoesntExist(string authorName, string authorEmail)
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // var context = null;

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);

            // Act
            ar.CreateAuthor(authorName, authorEmail);
            await context.SaveChangesAsync();


            // Assert
            var retrievedAuthor = ar.GetAuthorByName(authorName);
            Assert.Equal(authorName, retrievedAuthor.Name);
            Assert.Equal(authorEmail, retrievedAuthor.Email);
        }
    }

    [Theory]
    [InlineData("test1", "test1@test.dk")]
    [InlineData("test2", "test2@test.de")]
    public async Task CreateAuthorInDatabase_DoesExist(string authorName, string authorEmail)
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // var context = null;

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

    [Theory]
    [InlineData("test1", "test1@test.dk", "This is a test cheep1")]
    [InlineData("test2", "test2@test.de", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_AuthorExists(string authorName, string authorEmail, string message) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // var context = null;

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
            Assert.Equal(authorName, retrievedAuthor.Name);
            Assert.Equal(message, retrievedAuthor.Cheeps[0].Text);
        }
    }

    [Theory]
    [InlineData("test1", "This is a test cheep1")]
    [InlineData("test2", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_AuthorDoesntExist(string authorName, string message) 
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // var context = null;

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            AuthorRepository ar = new AuthorRepository(context);
            CheepRepository cr = new CheepRepository(context);

            // Assert
            Assert.Throws<Exception>(() => cr.CreateCheep(ar.GetAuthorByName(authorName), message));
        }
    }

    [Theory]
    [InlineData("test1", "test1@test.dk", "This is a test cheep1")]
    [InlineData("test2", "test2@test.de", "This is a test cheep2")]
    public async Task CreateCheepInDatabase_CreateAuthorAfterException(string authorName, string authorEmail, string message) 
    {
        //TODO Is this more of an intergration test?
        // This test shows how we should use the different methods to properly create cheeps
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // var context = null;

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