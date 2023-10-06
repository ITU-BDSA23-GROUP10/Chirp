using System.Net;
using System.Net.Http.Json;
using SimpleDB;

namespace Chirp.CLI.test;
public class ChirpCliHttpClient_UnitTests
{
    private readonly HttpClient client;
        
    public ChirpCliHttpClient_UnitTests()
    {
        client = new HttpClient
        {
            BaseAddress = new Uri("https://bdsagroup10chirprazor.azurewebsites.net/")
        };
    }

    // This tests if the expected base url and actual azure server url are the same
    [Fact]
    public async Task HttpClient_IsCorrectlyConfigured()
    {
        //arrange
        var expectedBaseAddress = new Uri("https://bdsagroup10chirprazor.azurewebsites.net");
        
        //act
        var actualBaseAdress = client.BaseAddress;
        
        //assert
        Assert.Equal(expectedBaseAddress, actualBaseAdress);
    }

    //this tests to see whether or not the endpoint /cheeps works from the web app url
    [Fact]
    public async Task HttpClient_IsItCorrectEndpoint_OnGetCheeps()
    {
        //arrange
        var expectedEndpoint = "/";
        
        //act
        var response = await client.GetAsync(expectedEndpoint);

        //assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    //this tests to see whether or not the endpoint /cheeps has data to be read from the web app url
    [Fact]
    public async Task HttpClient_IsThereDatatoGet_OnGetCheeps()
    {
        //arrange
        var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        
        //act
        var cheeps = await response.Content.ReadAsStringAsync();
        
        //assert
        Assert.NotEmpty(cheeps);
        
    }

    //this tests to see whether or not we can post to the endpoint /cheep on the web app url
    [Fact]
    public async Task HttpClient_IsItCorrectEndpoint_OnPostCheep()
    {
        //arrange
        var expectedEndpoint = "/cheep";
        var cheep = new Cheep
        {
            Message = "test cheep from HttpClient_IsItCorrectEndpoint_OnPostCheep method in chirpclihttpclient_unittests",
            Author = "Test Author",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        //act
        var response = await client.PostAsJsonAsync(expectedEndpoint,cheep);
    
        //assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}