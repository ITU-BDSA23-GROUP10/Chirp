using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Web;
using SimpleDB;
using Chirp.CLI;
using Xunit;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Text;

namespace Chirp.CLI.test;

public class endToEndTest
{
    [Fact]
    public void readCheeps_EndtoEnd_Test()
    {
        //Arrange
        string[] Cheeps;
        string cmd = "read";

        //Act
        Cheeps = start_Process_For_Client_CLI(cmd).Replace("\r", "").Split("\n");

        // Assert
        Assert.True(
            Cheeps.Contains("ropf @ 01/08/2023 14.09.20: Hello, BDSA students!") ||
            Cheeps.Contains("ropf @ 01-08-2023 14:09:20: Hello, BDSA students!") ||
            Cheeps.Contains("ropf @ 08/01/2023 12:09:20: Hello, BDSA students!"));
        Assert.True(
            Cheeps.Contains("rnie @ 02/08/2023 14.19.38: Welcome to the course!") ||
            Cheeps.Contains("rnie @ 02-08-2023 14:19:38: Welcome to the course!") ||
            Cheeps.Contains("ropf @ 08/02/2023 12:09:20: Hello, BDSA students!"));
        Assert.True(
            Cheeps.Contains("rnie @ 02/08/2023 14.37.38: I hope you had a good summer.") ||
            Cheeps.Contains("rnie @ 02-08-2023 14:37:38: I hope you had a good summer.") ||
            Cheeps.Contains("rnie @ 08/02/2023 12:37:38: I hope you had a good summer."));
        Assert.True(
            Cheeps.Contains("ropf @ 02/08/2023 15.04.47: Cheeping cheeps on Chirp :)") ||
            Cheeps.Contains("ropf @ 02-08-2023 15:04:47: Cheeping cheeps on Chirp :)") ||
            Cheeps.Contains("ropf @ 08/02/2023 13:04:47: Cheeping cheeps on Chirp :)"));
    }

    [Fact]
    public void postCheep_And_Read_After_EndtoEnd_Test()
    {
        //Arrange
        string[] Cheeps;
        string cmd = "cheep \"This Is Test From The Endtoend Test\"";

        //Act
        start_Process_For_Client_CLI(cmd);
        cmd = "read";
        Cheeps = start_Process_For_Client_CLI(cmd).Replace("\r", "").Split("\n");


        //Assert
        Assert.True(Cheeps.Any(x => x.Contains("This Is Test From The Endtoend Test")));
    }


    private string start_Process_For_Client_CLI(string cmd)
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

    private readonly HttpClient client;

    public endToEndTest()
    {
        client = new HttpClient
        {
            BaseAddress = new Uri("https://bdsagroup10chirpremotedb.azurewebsites.net/")
        };
    }

    //https://stackoverflow.com/questions/37432999/get-content-result-from-httprequestmessage
    [Fact]
    public async void http_readCheeps_EndtoEnd_Test()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "cheeps");

        // Act
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(content.Contains("{\"author\":\"ropf\",\"message\":\"Hello, BDSA students!\",\"timestamp\":1690891760}"));
        Assert.True(content.Contains("{\"author\":\"rnie\",\"message\":\"Welcome to the course!\",\"timestamp\":1690978778}"));
        Assert.True(content.Contains("{\"author\":\"rnie\",\"message\":\"I hope you had a good summer.\",\"timestamp\":1690979858}"));
        Assert.True(content.Contains("{\"author\":\"ropf\",\"message\":\"Cheeping cheeps on Chirp :)\",\"timestamp\":1690981487}"));

    }

    [Fact]
    public async void http_postCheep_And_Read_After_EndtoEnd_Test()
    {

        // Arrange
        var newCheep = new Cheep();
        newCheep.Author = "END2END";
        newCheep.Message = "END2END is testing the server";

        var requestPost = new HttpRequestMessage(HttpMethod.Post, "cheep");
        var content = new StringContent(JsonConvert.SerializeObject(newCheep), Encoding.UTF8, "application/json");
        requestPost.Content = content;

        // Act
        var responsePost = await client.SendAsync(requestPost);
        var responseString = await HttpGetBodyAsync();


        //Assert.True(true);
        Assert.True(responseString.Contains("{\"author\":\"END2END\",\"message\":\"END2END is testing the server\","));
    }

    private async Task<string> HttpGetBodyAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "cheeps");
        var response = await client.SendAsync(request);

        return await response.Content.ReadAsStringAsync();

    }
}