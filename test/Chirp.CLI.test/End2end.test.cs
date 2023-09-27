using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimpleDB;
using Chirp.CLI;
using Xunit;
using System.Diagnostics;

namespace Chirp.CLI.test;

public class endToEndTest
{

    /*[Fact]
    public void endtoEnd1()
    {
        //Arrange
        string[] Cheeps;
        string cmd = "read";

        //Act
        Cheeps = check(cmd).Replace("\r", "").Split("\n");

        // Assert
        Assert.True(Cheeps.Contains("ropf @ 01/08/2023 14.09.20: Hello, BDSA students!"));
        Assert.True(Cheeps.Contains("rnie @ 02/08/2023 14.19.38: Welcome to the course!"));
        Assert.True(Cheeps.Contains("rnie @ 02/08/2023 14.37.38: I hope you had a good summer."));
        Assert.True(Cheeps.Contains("ropf @ 02/08/2023 15.04.47: Cheeping cheeps on Chirp :)"));
    }

    [Fact]
    public void endtoEnd2()
    {
        //Arrange
        string[] Cheeps;
        string cmd = "cheep \"This Is Test From Alex the one and only, I'm so good at this shit\"";

        //Act
        check(cmd);
        cmd = "read";
        Cheeps = check(cmd).Replace("\r", "").Split("\n");
        

        //Assert
        Assert.True(Cheeps.Any(x => x.Contains("This Is Test From Alex the one and only, I'm so good at this shit")));
        //Alexa @ 22/09/2023 11.10.46: This Is Test From Alex the one and only, I'm so good at this shit
    }


    private string check(string cmd)
    {
        string output;
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run " + cmd;
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

*/
}