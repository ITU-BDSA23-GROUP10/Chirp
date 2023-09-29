using System.Net;
using Newtonsoft.Json;
using System.Text;
using SimpleDB;

namespace CSVDatabaseWebService.Tests
{
    public class CSVDatabaseTests
    {
        private readonly HttpClient client;
        
        public CSVDatabaseTests()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net/")
            };
        }

        [Fact]
        public async Task GetCheeps_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/cheeps");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostCheep_ShouldReturn200()
        {
            // Arrange
            var cheep = new Cheep { Author = "hejehj", Message = "test", Timestamp = 1695386940 };
            var content = new StringContent(JsonConvert.SerializeObject(cheep), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/cheep")
            {
                Content = content
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            //Post sends a 201 success
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        
        [Fact]
        public void SingletonDB_ShouldOnlyCreateOne()
        {
            // Arrange
            SingletonDB dbSingleton1 = SingletonDB.Instance;
            SingletonDB dbSingleton2 = SingletonDB.Instance;
            IDatabaseRepository<Cheep> db1 = dbSingleton1.Database;
            IDatabaseRepository<Cheep> db2 = dbSingleton2.Database;
            Cheep newCheep = new Cheep
            {
                Message = "Testing Singleton",
                Author = "Test Author",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Act
            db2.Store(newCheep);

            // Assert
            Assert.Equal(db1, db2);
        }
        
    }
}