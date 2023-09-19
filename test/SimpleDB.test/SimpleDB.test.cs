using SimpleDB;
using Xunit;
using System.Collections.Generic;

// basic tests
namespace SimpleDB.Tests
{
    public class CSVDatabaseTests
    {
        // [Fact]
        public void ReadMethod_ShouldReadData()
        {
            // // Arrange
            // var csvdb = new CSVDatabase<Cheep>();

            // // Act
            // var records = csvdb.Read();

            // // // Assert
            // Assert.NotEmpty(records);
        }

        // [Fact]
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
            // foreach(var cheep in cheepList) 
            // {
            //     csvdb.Store(cheep);
            // }

            // // Assert
            // var records = csvdb.Read();
            // Assert.Contains(records, record => record.Author == "karpe" && record.Message == "temp");
        }
    }
}