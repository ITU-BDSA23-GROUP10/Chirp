using Xunit;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChirpCLI.Tests
{
    public class ChirpCLITests
    {
        private readonly HttpClient client;
        
        public ChirpCLITests()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net/")
            };
        }

        // Integration tests
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
}