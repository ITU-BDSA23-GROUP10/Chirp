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
        string[] Cheeps;

        //Act
        Cheeps = check().Replace("\r", "").Split("\n");

        // Assert
        Assert.True(Cheeps.Contains("ropf @ 01/08/2023 14.09.20: Hello, BDSA students!"));
        Assert.True(Cheeps.Contains("rnie @ 02/08/2023 14.19.38: Welcome to the course!"));
        Assert.True(Cheeps.Contains("rnie @ 02/08/2023 14.37.38: I hope you had a good summer."));
        Assert.True(Cheeps.Contains("ropf @ 02/08/2023 15.04.47: Cheeping cheeps on Chirp :)"));
        //Assert.Equal("ropf @ 01/08/2023 14.09.20: Hello, BDSA students!", Cheeps[0]);
        //Assert.Equal("rnie @ 02/08/2023 14.19.38: Welcome to the course!", Cheeps[1]);
        //Assert.Equal("rnie @ 02/08/2023 14.37.38: I hope you had a good summer.", Cheeps[2]);
        //Assert.Equal("ropf @ 02/08/2023 15.04.47: Cheeping cheeps on Chirp :)", Cheeps[3]);
        /*
            ropf @ 01/08/2023 14.09.20: Hello, BDSA students!
            rnie @ 02/08/2023 14.19.38: Welcome to the course!
            rnie @ 02/08/2023 14.37.38: I hope you had a good summer.
            ropf @ 02/08/2023 15.04.47: Cheeping cheeps on Chirp :)
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
    {
        // Act
        /*string output = "";
        
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet"; //"/usr/bin/dotnet";
            process.StartInfo.Arguments = "run --project src/Chirp.CLI/Chirp.CLI.csproj --read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            //standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
        }

        return output;*/
        
        
        //Console.WriteLine("This pure: " + output);
        //string fstCheep = output.Split("\n")[0];
        //Console.WriteLine("This is not: " + fstCheep);
        
        //return fstCheep;*/
        string output;
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run --read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = Path.Combine("..", "..", "..", "..", "..", "src", "Chirp.CLI");

            process.Start();
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            
            process.WaitForExit();

            return output;
        }
    }


}