using Xunit;
using SimpleDB;
using System.Collections.Generic;

namespace Chirp.CLI.test;
public class UnitTest1
{/*
    private class StubDatabaseRepository : IDatabaseRepository<Cheep>
    {
        public List<Cheep> Cheeps = new List<Cheep>();

        public List<Cheep> Read(int? id)
        {
            return Cheeps;
        }

        public void Store(List<Cheep> records)
        {
            Cheeps.AddRange(records);
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

    [Fact]
    public void TestReadCheeps()
    {
        // Arrange
        var stubDb = new StubDatabaseRepository();
        var stubUi = new StubUserInterface();
        var testCheep = new Cheep
        {
            Message = "test message",
            Author = "jonas",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        stubDb.Cheeps.Add(testCheep);
        Program.db = stubDb;
        Program.ui = stubUi;

        // Act
        Program.ReadCheeps();

        // Assert
        Assert.Contains(testCheep, stubUi.CheepsToPrint);
    }

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
    }*/
}