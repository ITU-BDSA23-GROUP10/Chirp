
using SimpleDB;
public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}


public class CheepService : ICheepService
{
    DBFacade facadeDB = new DBFacade();

    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Rasmus", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public List<CheepViewModel> GetCheeps()
    {
        List<Cheep> cheeps = facadeDB.GetCheeps();
        List<CheepViewModel> cheepVM = new List<CheepViewModel>();

        foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepViewModel 
            (
                cheep.Author,
                cheep.Message,
                UnixTimeStampToDateTimeString(cheep.Timestamp)
            ));
        }

        return cheepVM;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        List<Cheep> cheeps = facadeDB.GetCheepsAuthorSQL(author);
        List<CheepViewModel> cheepVM = new List<CheepViewModel>();

       foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepViewModel 
            (
                cheep.Author,
                cheep.Message,
                cheep.Timestamp.ToString()
            ));
        } 

        // filter by the provided author name
        return cheepVM;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("dd/MM/yy H:mm:ss");
    }

}
