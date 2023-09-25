using SimpleDB;

// these tests should be refactored to fit http request standards
namespace Chirp.CLI.test;
public class UnitTest1
{
    //took info on how to do testing from
    //https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test

    private class TestDatabaseRepository : IDatabaseRepository<Cheep>
    {
        public List<Cheep> Cheeps = new List<Cheep>();

        //from IDatabaseRepository.cs
        public IEnumerable<Cheep> Read(int? id = null)
        {
            return Cheeps;
        }

        public void Store(Cheep record)
        {
            Cheeps.Add(record);
        }
    }

    private class TestUserInterface : UserInterface
    {
        public List<Cheep> CheepsToPrint;

        public void PrintCheeps(List<Cheep> cheeps)
        {
            CheepsToPrint = cheeps;
        }
    }

    [Fact]
    public void TestPostCheep()
    {
        // Arrange
        var testDb = new TestDatabaseRepository();
        Program.db = testDb;

        // Act
        Program.PostCheep("test message");

        // Assert
        Assert.Single(testDb.Cheeps);
        Assert.Equal("test message", testDb.Cheeps[0].Message);
    }
}