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

    /*[Fact]
    public void TestPostCheep()
    {
        // Arrange
        IEnumerable<Cheep> cheeps;

        // Act
        Program.PostCheep("test message");
        cheeps = Read();

        // Assert
        Assert.Single(testDb.Cheeps);
        Assert.Equal("test message", testDb.Cheeps[0].Message);
    }*/
}