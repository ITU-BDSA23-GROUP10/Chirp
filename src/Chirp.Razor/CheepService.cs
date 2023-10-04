
using SimpleDB;
public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    //page is for pagination
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
    Task<int> GetCount();
    Task<int> GetCount(string author);
    int GetLimit();
}


public class CheepService : ICheepService
{
    DBFacade facadeDB = new DBFacade();
    public readonly int limit = 32;

    public async Task<int> GetCount()
    {
        return await facadeDB.CountCheeps();
    }

    public async Task<int> GetCount(string author)
    {
        return await facadeDB.CountCheeps(author);
    }

    public int GetLimit()
    {
        return limit;
    }
    public List<CheepViewModel> GetCheeps(int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page -1) * limit;
        //pagination end

        List<Cheep> cheeps = facadeDB.GetCheeps(offset, limit);
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

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        List<Cheep> cheeps = facadeDB.GetCheepsAuthorSQL(author, offset, limit);
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
