using Chirp.Infrastructure;
using SimpleDB;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Razor;
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

    public (List<CheepDTO>, int CheepsCount) GetCheeps(int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        var (Cheeps, CheepsCount) = facadeDB.GetCheeps(offset, limit);

        //If cheeps list is empty just give it an empty list to ensure that the site actually has a list
        if (Cheeps == null)
        {
            return (new List<CheepDTO>(), 0);
        } 

        // filter by the provided author name
        return (Cheeps, CheepsCount);
    }

    public (List<CheepDTO>, int CheepsCount) GetCheepsFromAuthor(string author, int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        var (Cheeps, AuthorsCheepsCount) = facadeDB.GetCheepsByAuthor(author, offset, limit);

        //If cheeps list is empty just give it an empty list to ensure that the site actually has a list
        if (Cheeps == null)
        {
            return (new List<CheepDTO>(), 0);
        }

        // filter by the provided author name
        return (Cheeps, AuthorsCheepsCount);
    }

    //Changes unix time stamps to be the european date month year standard
    /*private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString();
    }*/
}