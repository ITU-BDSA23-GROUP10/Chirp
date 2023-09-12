using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to Chirp. Chirp is a the platform formerly known as twitter clone developed by 5 idiots at ITU. Enjoy!\n");
    Console.WriteLine("Usage:\n\tTo read cheeps (tweeds) type 'dotnet run --read'\n" +
                        "\tTo right a cheep type 'dotnet run -- cheep <message>'");
    return;
}


if (args[0] == "read")
{
    // Read datafile with CsvHelper
    // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
    using (var reader = new StreamReader(@"data/chirp_cli_db.csv"))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        var records = csv.GetRecords<Cheep>();
        // Show cheeps in console
        foreach (var Cheep in records)
        {
            // Convert from unix timestamp to DateTime for local time
            // Code adapted from Stackoverflow answer: https://stackoverflow.com/a/250400
            DateTime Cheeptime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Cheeptime = Cheeptime.AddSeconds(Cheep.Timestamp).ToLocalTime();
            
            Console.Write($"{Cheep.Author} @ {Cheeptime}: {Cheep.Message}\n");
        }
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
    
    // Write to new cheep to csv file using CsvHelper
    // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        // Don't write the header again.
        HasHeaderRecord = false,
    };

    using (var stream = File.Open("data/chirp_cli_db.csv", FileMode.Append))
    using (var writer = new StreamWriter(stream))
    using (var csv = new CsvWriter(writer, config))
    {
        csv.WriteRecords(records);
    }
}
public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}