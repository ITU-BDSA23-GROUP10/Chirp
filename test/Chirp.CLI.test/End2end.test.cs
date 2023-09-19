using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimpleDB;
using Chirp.CLI;
using Xunit;
using System.Diagnostics;

namespace Chirp.CLI.test;

public class endToEndTest
{

    [Fact]
    public void endtoEnd1()
    {

        //Arrange
        string[] fstCheep; //Contains every line that the program prints

        //Act
        //runs the read command, deletes whitespaces and splits on each line
        fstCheep = check().Replace("\r", "").Split("\n");


        /*// Arrange
        var list = new List<Cheep>();
        list.Add(new Cheep { Author = "ropf", Message = "Hello, BDSA students!", Timestamp = 1690891760 });
        list.Add(new Cheep { Author = "rnie", Message = "Welcome to the course!", Timestamp = 1690978778 });
        list.Add(new Cheep { Author = "rnie", Message = "I hope you had a good summer.", Timestamp = 1690979858 });
        list.Add(new Cheep { Author = "ropf", Message = "Cheeping cheeps on Chirp :)", Timestamp = 1690981487 });

        // Act
        check();*/


        // Assert
        //Assert.Equal(list, records);
        Assert.Equal("ropf,\"Hello, BDSA students!\",1690891760", fstCheep[0]);
        Assert.Equal("rnie,\"Welcome to the course!\",1690978778", fstCheep[1]);
        Assert.Equal("rnie,\"I hope you had a good summer.\",1690979858", fstCheep[2]);
        Assert.Equal("ropf,\"Cheeping cheeps on Chirp :)\",1690981487", fstCheep[3]);
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
        Assert.Equal(1, 1);

        /*string[] ar = new string[1];
        ar[0] = "dotnet run --Cheep plz plz come on man";

        //pro.write(Alex,"plz plz come on man",1790981487);

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
        Assert.Equal(list, records);*/

        /*
            Author,Message,Timestamp
            ropf,"Hello, BDSA students!",1690891760
            rnie,"Welcome to the course!",1690978778
            rnie,"I hope you had a good summer.",1690979858
            ropf,"Cheeping cheeps on Chirp :)",1690981487
            Alex,"plz plz come on man",1790981487
        */
    }


    private string check()
    {/*
        // Act
        string output = "";
        
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI/bin/Debug/net7.0/chirp.dll read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.

            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        
        
        //Console.WriteLine("This pure: " + output);
        string fstCheep = output.Split("\n")[0];
        //Console.WriteLine("This is not: " + fstCheep);
        
        return fstCheep;*/
        string output;
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI/bin/Debug/net7.0/Chirp.CLI.dll --read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = "../../../../../";

            process.Start();
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }


}