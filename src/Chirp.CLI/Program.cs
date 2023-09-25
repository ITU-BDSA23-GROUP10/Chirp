using SimpleDB;
using System.CommandLine;
using System.CommandLine.Parsing;


using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chirp;
public class Program
{

    static readonly SingletonDB dbSingleton = SingletonDB.Instance;
    static readonly IDatabaseRepository<Cheep> db = dbSingleton.Database;
    private static UserInterface ui = new UserInterface();


    static async Task Main(string[] args)
    {
        // Create the posible commands for the program
        // https://learn.microsoft.com/en-us/dotnet/standard/commandline/define-commands
        var rootCommand = new RootCommand();
        var readCommand = new Command("read", "Read all the cheeps");
        rootCommand.Add(readCommand);
        var readLimit = new Argument<int?>
            (name: "limit",
            description: "A limit on the amount cheeps read",
            getDefaultValue: () => null);
        readCommand.Add(readLimit);
        var cheepCommand = new Command("cheep", "Write a new cheep");
        rootCommand.Add(cheepCommand);
        var messageArgument = new Argument<string>
            (name: "message",
            description: "The message in the cheep",
            getDefaultValue: () => "");
        cheepCommand.Add(messageArgument);

        readCommand.SetHandler((readLimitValue) =>
            {
                ReadCheeps(readLimitValue);
            },
            readLimit);
        
        cheepCommand.SetHandler((messageArgumentValue) =>
            {
                PostCheep(messageArgumentValue + "");
            },
            messageArgument);

        await rootCommand.InvokeAsync(args);
    }

public record chiptest(string Author, string Message, long Timestamp);

    public async static void ReadCheeps(int? limit = null) 
    {
        
        // Create an HTTP client object
        var baseURL = "http://localhost:5076";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
        // JSON object in the body of the response
        var cheep = await client.GetFromJsonAsync<Cheep>("cheeps");
        
        Console.WriteLine("test");
        //ui.PrintCheeps(cheep);
        Console.WriteLine(cheep.Author);

    /*
    private readonly HttpClient client;
        
        [Fact]
        public async Task GetCheeps_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/cheeps");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    
    
    */
        //var cheeps = db.Read(limit);
        //ui.PrintCheeps(cheeps); 
    }

    public static void PostCheep(string message) 
    {
        var newCheep = new Cheep
        {
            Message = message,
            Author = Environment.UserName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        db.Store(newCheep);
    }
}