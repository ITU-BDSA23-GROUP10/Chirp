using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimpleDB;
using Chirp.CLI;
using Xunit.Sdk;

namespace Chirp.CLI.test;

public class endToEndTest
{

    [Fact]
    public void endtoEnd1()
    {
        // Arrange
        string[] ar = new string[1];
        ar[0] = "dotnet run --read";

        Program pro = new Program();
        pro.Main(ar);

        var csv = new CSVDatabase<Cheep>();

        var list = new List<Cheep>();
        list.Add(new Cheep { Author = "ropf", Message = "Hello, BDSA students!", Timestamp = 1690891760 });
        list.Add(new Cheep { Author = "rnie", Message = "Welcome to the course!", Timestamp = 1690978778 });
        list.Add(new Cheep { Author = "rnie", Message = "I hope you had a good summer.", Timestamp = 1690979858 });
        list.Add(new Cheep { Author = "ropf", Message = "Cheeping cheeps on Chirp :)", Timestamp = 1690981487 });

        // Act
        var records = csv.Read();



        // Assert
        Assert.Equal(list, records);

        /*
            Author,Message,Timestamp
            ropf,"Hello, BDSA students!",1690891760
            rnie,"Welcome to the course!",1690978778
            rnie,"I hope you had a good summer.",1690979858
            ropf,"Cheeping cheeps on Chirp :)",1690981487
        */
    }

    [Fact]
    public void endtoEnd2()
    {
        string[] ar = new string[1];
        ar[0] = "dotnet run --Cheep plz plz come on man";

        Program pro = new Program();
        pro.Main(ar);

        var csv = new CSVDatabase<Cheep>();

        var list = new List<Cheep>();
        list.Add(new Cheep { Author = "ropf", Message = "Hello, BDSA students!", Timestamp = 1690891760 });
        list.Add(new Cheep { Author = "rnie", Message = "Welcome to the course!", Timestamp = 1690978778 });
        list.Add(new Cheep { Author = "rnie", Message = "I hope you had a good summer.", Timestamp = 1690979858 });
        list.Add(new Cheep { Author = "ropf", Message = "Cheeping cheeps on Chirp :)", Timestamp = 1690981487 });
        list.Add(new Cheep { Author = "Alex", Message = "plz plz come on man", Timestamp = 1790981487 });

        // Act
        var records = csv.Read();

        // Assert
        Assert.Equal(list, records);

        /*
            Author,Message,Timestamp
            ropf,"Hello, BDSA students!",1690891760
            rnie,"Welcome to the course!",1690978778
            rnie,"I hope you had a good summer.",1690979858
            ropf,"Cheeping cheeps on Chirp :)",1690981487
            Alex,"plz plz come on man",1790981487
        */
    }


}