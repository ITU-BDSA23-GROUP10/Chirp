using SimpleDB;
using Xunit;
using System.Collections.Generic;

// basic tests
namespace SimpleDB.Tests
{
    public class CSVDatabaseTests
    {
        //[Fact]
        public void ReadMethod_ShouldReadData()
        {
            // // Arrange
            // var csvdb = new CSVDatabase<Cheep>();

            // // Act
            // var records = csvdb.Read();

            // // Assert
            // Assert.NotEmpty(records);
        }

        //[Fact]
        public void StoreMethod_ShouldStoreData()
        {
            // // Arrange
            // var csvdb = new CSVDatabase<Cheep>();
            // var cheepList = new List<Cheep>
            // {
            //     //placeholder
            //     new Cheep { Author = "karpe", Message = "temp", Timestamp = 1694524141 }
            // };

            // // Act
            // csvdb.Store(cheepList);

            // // Assert
            // var records = csvdb.Read();
            // Assert.Contains(records, record => record.Author == "karpe" && record.Message == "temp");
        }

        [Fact]
        public void CSVDbSingleton_ShouldOnlyCreateOne()
        {
            // Arrange
            CSVDbSingleton dbSingleton1 = CSVDbSingleton.Instance;
            CSVDbSingleton dbSingleton2 = CSVDbSingleton.Instance;

            IDatabaseRepository<Cheep> db1 = dbSingleton1.Database;
            IDatabaseRepository<Cheep> db2 = dbSingleton2.Database;

            Cheep newCheep = new Cheep
            {
                Message = "Testing Singleton",
                Author = Environment.UserName,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Act
            db2.Store(newCheep);

            // Assert
            Assert.Equal(db1, db2);
        }
    }
}