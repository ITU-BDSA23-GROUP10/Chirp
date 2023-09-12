using SimpleDB;

IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();   

UserInterface ui = new UserInterface();

if (args.Length == 0)
{
    Console.WriteLine("Welcome to Chirp. Chirp is a the platform formerly known as twitter clone developed by 5 idiots at ITU. Enjoy!\n");
    Console.WriteLine("Usage:\n\tTo read cheeps (tweeds) type 'dotnet run --read'\n" +
                        "\tTo right a cheep type 'dotnet run -- cheep <message>'");
    return;
}


if (args[0] == "read")
{
    var cheeps = db.Read();
    ui.PrintCheeps(cheeps);
    
}
else if (args[0] == "cheep")
{
    var Message = args[1];

    // New write with CsvHelper
    var newCheep = new Cheep
    {
        Message = Message,
        Author = Environment.UserName,
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };
    var records = new List<Cheep> {newCheep};
    db.Store(records);
}