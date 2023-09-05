using Microsoft.VisualBasic.FileIO;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to Chirp. Chirp is a the platform formerly known as twitter clone developed by 5 idiots at ITU. Enjoy!\n");
    Console.WriteLine("Usage:\n\tTo read cheeps (tweeds) type 'dotnet run --read'\n" +
                        "\tTo right a cheep type 'dotnet run -- cheep <message>'");
    return;
}

if (args[0] == "read")
{
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
            string Name = fields[0];
            string Message = fields[1];
            // Convert from unix timestamp to DateTime for local time
            // Code adapted from Stackoverflow answer: https://stackoverflow.com/a/250400
            DateTime Cheeptime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Cheeptime = Cheeptime.AddSeconds(int.Parse(fields[2])).ToLocalTime();
            
            Cheeps.Add((Name, Message, Cheeptime));
        }
    }

    // Show cheeps in console
    foreach (var Cheep in Cheeps)
    {
        Console.Write($"{Cheep.Item1} @ {Cheep.Item3}: {Cheep.Item2}\n");
    }
}
else if (args[0] == "cheep")
{
    var Message = args[1];
    
}