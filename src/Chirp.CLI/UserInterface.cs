using SimpleDB;

public class UserInterface
{    
    public void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            // Convert from unix timestamp to DateTime for local time
            // Code adapted from Stackoverflow answer: https://stackoverflow.com/a/250400
            DateTime cheeptime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            cheeptime = cheeptime.AddSeconds(cheep.Timestamp).ToLocalTime();
                
            Console.Write($"{cheep.Author} @ {cheeptime}: {cheep.Message}\n");
        }
    }    
}