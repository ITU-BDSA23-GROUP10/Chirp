using SimpleDB;
using ClientToServer;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace Chirp;

public class Program
{

    private static UserInterface ui = new UserInterface();
    private static IServerInteraction<Cheep> client = new ClientServerInteraction<Cheep>("https://bdsagroup10chirpremotedb.azurewebsites.net");

    public static async Task Main(string[] args)
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

        readCommand.SetHandler(async (readLimitValue) =>
        {
            await ReadCheeps(readLimitValue);
        },
        readLimit);  
        
        cheepCommand.SetHandler(async (messageArgumentValue) =>
        {
            await PostCheep(messageArgumentValue + "");
        },
        messageArgument);

        await rootCommand.InvokeAsync(args);
    }

    //mostly from session 4 slide (class BDSA)
    //remember to always call the method asyncronously in the readCommand.SetHandler (else it fails)
    public static async Task ReadCheeps(int? limit = null) 
    {
        //Our requests for data should expect JSON
        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        try
        {
            var json = await client.GetToEndpointWithJsonResponceAsync("cheeps");

            //deserializes the JSON string stored in the "json" variable into a list of Cheep objects
            var cheeps = JsonSerializer.Deserialize<List<Cheep>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //print it
            ui.PrintCheeps(cheeps);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task PostCheep(string message) 
    {   
        // Check if valid message
        if (IsMSGEmptyOrStartsWithAt(message)) return;

        var newCheep = new Cheep
        {
            Message = message,
            Author = Environment.UserName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        try
        {
            var result = await client.PostToEndpointWithJsonResponceAsync("cheep", newCheep);
            //Write the server response to posting new data
            Console.WriteLine($"Response from server: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static bool IsMSGEmptyOrStartsWithAt(string message)
    {
        if (string.IsNullOrEmpty(message) || message.StartsWith("@"))
        {
            Console.WriteLine("Chirp! Cheeps cannot be empty or start with @.\nPlease try again :)");
            return true;
        }
        return false;
    }
    
}