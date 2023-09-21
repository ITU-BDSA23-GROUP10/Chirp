using SimpleDB;
using System.CommandLine;
using System.CommandLine.Parsing;
//asdsadsadsdsadadsadsadada
public class Program
{
    static readonly CSVDbSingleton dbSingleton = CSVDbSingleton.Instance;
    static readonly IDatabaseRepository<Cheep> db = dbSingleton.Database;
    private static UserInterface ui = new UserInterface();

    static async Task Main(string[] args)
    {
        // tetasastsatsatsats
        // Create the posible commands for the program
        // https://learn.microsoft.com/en-us/dotnet/standard/commandline/define-commands
        var rootCommand = new RootCommand();
        var readCommand = new Command("read", "Read all the cheeps");
        rootCommand.Add(readCommand);
        var cheepCommand = new Command("cheep", "Write a new cheep");
        rootCommand.Add(cheepCommand);
        var messageArgument = new Argument<string>
            (name: "message",
            description: "The message in the cheep",
            getDefaultValue: () => "");
        cheepCommand.Add(messageArgument);

        readCommand.SetHandler(() =>
            {
                ReadCheeps();
            });
        
        cheepCommand.SetHandler((messageArgumentValue) =>
            {
                PostCheep(messageArgumentValue + "");
            },
            messageArgument);

        await rootCommand.InvokeAsync(args);
    }

    static void ReadCheeps() 
    {
        var cheeps = db.Read();
        ui.PrintCheeps(cheeps); 
    }

    static void PostCheep(string message) 
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