using Microsoft.VisualBasic.FileIO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Expressions;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to Chirp. Chirp is a the platform formerly known as twitter clone developed by 5 idiots at ITU. Enjoy!\n");
    Console.WriteLine("Usage:\n\tTo read cheeps (tweeds) type 'dotnet run --read'\n" +
                        "\tTo right a cheep type 'dotnet run -- cheep <message>'");
    return;
}


if (args[0] == "read")
{
    // New code with CsvHelper
    using (var reader = new StreamReader(@"data/chirp_cli_db.csv"))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        var records = csv.GetRecords<Cheep>();
        foreach (var record in records)
        {
            Console.WriteLine(record.Message);
        }
    }


    /*
    // Reading chirp csv file to store them in a list. 
    // Adapted form Stackoverflow answer: https://stackoverflow.com/a/33796861
    var Cheeps = new List<(string, string, DateTime)>();
    using (TextFieldParser csvParser = new TextFieldParser(@"data/chirp_cli_db.csv"))
    {
        csvParser.CommentTokens = new string[] { "#" };
        csvParser.SetDelimiters(new string[] { "," });
        csvParser.HasFieldsEnclosedInQuotes = true;

        // Skip the row with the column names
        csvParser.ReadLine();

        while (!csvParser.EndOfData)
        {
            // Read current line fields, pointer moves to the next line.
            string[] fields = csvParser.ReadFields();
            if (fields != null)
            {
                string Name = fields[0];
                string Message = fields[1];
                // Convert from unix timestamp to DateTime for local time
                // Code adapted from Stackoverflow answer: https://stackoverflow.com/a/250400
                DateTime Cheeptime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Cheeptime = Cheeptime.AddSeconds(int.Parse(fields[2])).ToLocalTime();
                
                Cheeps.Add((Name, Message, Cheeptime));
            }
        }
    }

    // Show cheeps in console
    foreach (var Cheep in Cheeps)
    {
        Console.Write($"{Cheep.Item1} @ {Cheep.Item3}: {Cheep.Item2}\n");
    }
    */
}
else if (args[0] == "cheep")
{
    var Message = args[1];

    // Opens CSV data file in 'Append mode'
    // code taken from https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-7.0
    using (StreamWriter Writer = new StreamWriter(@"data/chirp_cli_db.csv", append: true))
    {
        // Get system name
        var Name = Environment.UserName;
        // Stackoverflow answer: https://stackoverflow.com/a/35425123
        var Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); 
        Writer.WriteLine($"{Name}, \"{Message}\", {Time}");
    }
}
public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public int Timestamp { get; set; }
}