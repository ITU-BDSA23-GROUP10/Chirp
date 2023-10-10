using SimpleDB.Models;
using SimpleDB;

namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    //page is for pagination
    public (List<CheepViewModel>, int CheepsCount) GetCheeps(int page);
    public (List<CheepViewModel>, int CheepsCount) GetCheepsFromAuthor(string author, int page);
    int GetLimit();
}


public class CheepService : ICheepService
{
    protected readonly DBFacade facadeDB;
    
    public readonly int limit = 32;

    public CheepService(ChirpDBContext dBContext)
    {
        facadeDB = new DBFacade(dBContext);
    }

    public int GetLimit()
    {
        return limit;
    }

    public (List<CheepViewModel>, int CheepsCount) GetCheeps(int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        var (Cheeps, CheepsCount) = facadeDB.GetCheeps(offset, limit);
        List<Cheep> cheeps = Cheeps;

        List<CheepViewModel> cheepVM = new List<CheepViewModel>();
        foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepViewModel 
            (
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp.ToString()
            ));
        } 

        // filter by the provided author name
        return (cheepVM, CheepsCount);
    }

    public (List<CheepViewModel>, int CheepsCount) GetCheepsFromAuthor(string author, int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        var (Cheeps, AuthorsCheepsCount) = facadeDB.GetCheepsByAuthor(author, offset, limit);

        if (Cheeps == null)
        {
            return (new List<CheepViewModel>(), 0);
        }

        List<Cheep> cheeps = Cheeps;

        List<CheepViewModel> cheepVM = new List<CheepViewModel>();
        foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepViewModel 
            (
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp.ToString())
            );
        } 

        // filter by the provided author name
        return (cheepVM, AuthorsCheepsCount);
    }

    /*private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString();
    }*/

}