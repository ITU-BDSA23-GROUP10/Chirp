using SimpleDB;
using System.CommandLine;
using System.CommandLine.Parsing;
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
                PostCheep(messageArgumentValue ?? "");
            },
            messageArgument);

        await rootCommand.InvokeAsync(args);
    }

    static void ReadCheeps(int? limit = null) 
    {
        var cheeps = db.Read(limit);
        ui.PrintCheeps(cheeps); 
    }

    static void PostCheep(string message) 
    {
        if (IsMSGEmptyOrStartsWithAt(message)) return;

        var newCheep = new Cheep
        {
            Message = message,
            Author = Environment.UserName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        db.Store(newCheep);
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