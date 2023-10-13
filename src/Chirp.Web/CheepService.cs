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
        List<Cheep> cheeps = Cheeps;

        List<CheepDTO> cheepVM = new List<CheepDTO>();
        foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepDTO 
            (
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp.ToString()
            ));
        } 

        // filter by the provided author name
        return (cheepVM, CheepsCount);
    }

    public (List<CheepDTO>, int CheepsCount) GetCheepsFromAuthor(string author, int page)
    {
        //pagination start
        //int limit = 32;
        int offset = (page - 1) * limit;
        //pagination end

        var (Cheeps, AuthorsCheepsCount) = facadeDB.GetCheepsByAuthor(author, offset, limit);

        if (Cheeps == null)
        {
            return (new List<CheepDTO>(), 0);
        }

        List<Cheep> cheeps = Cheeps;

        List<CheepDTO> cheepVM = new List<CheepDTO>();
        foreach(Cheep cheep in cheeps) 
        {
            cheepVM.Add(new CheepDTO 
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