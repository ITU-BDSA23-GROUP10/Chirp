using SimpleDB;

namespace Chirp.CLI.test;
public class UnitTest1
{
    //took info on how to do testing from
    //https://learn.microsoft.com/en-us/visualstudio/test/using-stubs-to-isolate-parts-of-your-application-from-each-other-for-unit-testing?view=vs-2022&tabs=csharp
    //https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test

    private class StubDatabaseRepository : IDatabaseRepository<Cheep>
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

    private class StubUserInterface : UserInterface
    {
        public List<Cheep> CheepsToPrint;

        public void PrintCheeps(List<Cheep> cheeps)
        {
            CheepsToPrint = cheeps;
        }
    }

    /*
    TODO: currently fixing (jonas).. currently value is null probably on CheepsToPrint


    [Fact]
    public void TestReadCheeps()
    {
        // Arrange
        var stubDb = new StubDatabaseRepository();
        var stubUi = new StubUserInterface();
        var testCheep = new Cheep
        {
            Message = "Cheeping cheeps on Chirp :)",
            Author = "ropf",
            //Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        stubDb.Cheeps.Add(testCheep);
        Program.db = stubDb;
        Program.ui = stubUi;

        // Act
        Program.ReadCheeps();

        // Assert
        Assert.Contains(testCheep, stubUi.CheepsToPrint);
    }*/

    [Fact]
    public void TestPostCheep()
    {
        // Arrange
        var stubDb = new StubDatabaseRepository();
        Program.db = stubDb;

        // Act
        Program.PostCheep("test message");

        // Assert
        Assert.Single(stubDb.Cheeps);
        Assert.Equal("test message", stubDb.Cheeps[0].Message);
    }
}