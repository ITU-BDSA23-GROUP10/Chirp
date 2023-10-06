using System.Net;
using System.Net.Http.Json;
//using SimpleDB;

namespace Chirp.CLI.test;
public class ChirpCliHttpClient_UnitTests
{
    /*private readonly HttpClient client;
        
    public ChirpCliHttpClient_UnitTests()
    {
        client = new HttpClient
        {
            BaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net/")
        };
    }

    // This tests if the expected base url and actual azure server url are the same
    [Fact]
    public async Task HttpClient_IsCorrectlyConfigured()
    {
        //arrange
        var expectedBaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net");
        
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
        
    }*/

}