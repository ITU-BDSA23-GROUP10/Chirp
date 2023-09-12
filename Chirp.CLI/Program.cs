using SimpleDB;
using DocoptNet;


    IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();  

    UserInterface ui = new UserInterface();
// This is taken from this website
// https://docopt.github.io/docopt.net/dev/#api
    const string help = @"Chirp

Usage:
  --read
  --cheep <argument>
  -help

Options:
  -help                 This shows the help screen.
  --read                This prints all the cheeps.
  --cheep <message>     This will post a cheep.
 
";
static int Run(IDictionary<string, ArgValue> arguments)
{
    foreach (var (key, value) in arguments)
        Console.WriteLine("{0} = {1}", key, value);
    return 0;
}

static int OnError(string usage) { Console.Error.WriteLine(usage); return 1; }
static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
// var parser = Docopt.CreateParser(help)
//              //.WithVersion("")
//              .Parse(args)
//              .Match(Run,
//                     result => ShowHelp(result.Help),
//                     result => OnError(result.Usage));
var parser = Docopt.CreateParser(help);
//This is the correct way to do it but apparently args is not the right type...
var result = parser.Parse(args);

IDictionary<string, ArgValue> dictionary = result;
return 0;





// if (args.Length == 0)
// {
//     Console.WriteLine("Welcome to Chirp. Chirp is a the platform formerly known as twitter clone developed by 5 idiots at ITU. Enjoy!\n");
//     Console.WriteLine("Usage:\n\tTo read cheeps (tweeds) type 'dotnet run --read'\n" +
//                         "\tTo right a cheep type 'dotnet run -- cheep <message>'");
//     return;
// }
static int readCheeps(IDatabaseRepository<Cheep> db, UserInterface ui) 
{
    var cheeps = db.Read();
    ui.PrintCheeps(cheeps); 
    return 0;
}

static int postCheep(IDatabaseRepository<Cheep> db, string Message) 
{
    //New write with CsvHelper
    var newCheep = new Cheep
    {
        Message = Message,
        Author = Environment.UserName,
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };
    var records = new List<Cheep> {newCheep};
    db.Store(records);
    return 0;
}

// if (args[0] == "read")
// {
//     var cheeps = db.Read();
//     ui.PrintCheeps(cheeps);
    
// }
// else if (args[0] == "cheep")
// {
//     var Message = args[1];

//     // New write with CsvHelper
//     var newCheep = new Cheep
//     {
//         Message = Message,
//         Author = Environment.UserName,
//         Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
//     };
//     var records = new List<Cheep> {newCheep};
//     db.Store(records);
// }