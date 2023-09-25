using SimpleDB;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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

        /*readCommand.SetHandler((readLimitValue) =>
            {
                ReadCheeps(readLimitValue);
            },
            readLimit);*/

          readCommand.SetHandler(async (readLimitValue) =>
            {
                await ReadCheeps(readLimitValue);
            },
            readLimit);  
        
        /*cheepCommand.SetHandler((messageArgumentValue) =>
            {
                PostCheep(messageArgumentValue + "");
            },
            messageArgument);*/

        await rootCommand.InvokeAsync(args);
    }


    public static async Task ReadCheeps(int? limit = null) 
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri("http://localhost:5076");

        try
        {
            Console.WriteLine("Trying to fetch cheeps from the server...");

            // Send the request and get the response
            var response = await client.GetAsync("cheeps");
            Console.WriteLine($"Response status code: {response.StatusCode}");
            Console.WriteLine("Response headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            // Throw an exception if the response indicates an unsuccessful status code
            response.EnsureSuccessStatusCode();

            // Read the response as a string motherfucker you better work now
            var json = await response.Content.ReadAsStringAsync();

            // Log the JSON to the console
            Console.WriteLine($"JSON from server: {json}");

            // Attempt to deserialize the JSON
            var cheeps = JsonSerializer.Deserialize<List<Cheep>>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("Fetched cheeps from the server.");

            if (cheeps == null || cheeps.Count == 0)
            {
                Console.WriteLine("No Cheeps found.");
                return;
            }

            if(limit.HasValue && cheeps.Count > limit.Value)
            {
                cheeps = cheeps.Take(limit.Value).ToList();
            }
            ui.PrintCheeps(cheeps);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    /*public static async Task PostCheep(string message) 
    {

        var newCheep = new Cheep
        {
            Message = message,
            Author = Environment.UserName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri("http://localhost:5076");

        try
        {
            var response = await client.PostAsJsonAsync("cheep", newCheep);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response from server: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    /*public async static void ReadCheeps(int? limit = null) 
    {
        
        var cheeps = db.Read(limit);
        ui.PrintCheeps(cheeps); 
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
    }*/
    
}