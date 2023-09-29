
namespace Chirp.CLI.test;
public class ChirpCliHttpClient_UnitTests
{
    private readonly HttpClient client;
        
    public ChirpCliHttpClient_UnitTests()
    {
        client = new HttpClient
        {
            BaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net/")
        };
    }

    // does the base url work and the same as the actual azure server url?
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
}
