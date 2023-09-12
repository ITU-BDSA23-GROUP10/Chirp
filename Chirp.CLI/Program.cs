using SimpleDB;

IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();   

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
    foreach (var Cheep in cheeps)
    {
        // Convert from unix timestamp to DateTime for local time
        // Code adapted from Stackoverflow answer: https://stackoverflow.com/a/250400
        DateTime Cheeptime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Cheeptime = Cheeptime.AddSeconds(Cheep.Timestamp).ToLocalTime();
            
        Console.Write($"{Cheep.Author} @ {Cheeptime}: {Cheep.Message}\n");
    }
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