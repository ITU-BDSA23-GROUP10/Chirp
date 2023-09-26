using SimpleDB;

// these tests should be refactored to fit http request standards
namespace Chirp.CLI.test;
public class UnitTest1
{
    //took info on how to do testing from
    //https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test
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
        IDatabaseRepository<Cheep> testDb = new CSVDatabase<Cheep>();

        // Act
        Program.PostCheep("test message");
        List li = testDb.Read.toList();

        // Assert   
        //Assert.Single(testDb.Read);
        //Assert.Equal("test message", testDb.Cheeps[0].Message);
    }
}   