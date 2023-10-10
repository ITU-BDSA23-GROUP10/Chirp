using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;
using System.Linq;
using SimpleDB.ChirpRepository;
using SimpleDB.Models;

namespace SimpleDB;


public class DBFacade 
{
    private readonly CheepRepository _cheepRepository;
    private readonly AuthorRepository _authorRepository;

    public DBFacade(ChirpDBContext dbContext)
    {
        _cheepRepository = new CheepRepository(dbContext);
        _authorRepository = new AuthorRepository(dbContext);
    }

    public (List<Cheep> Cheeps, int CheepsCount) GetCheeps(int offset, int limit) 
    {
        return ((List<Cheep>, int))_cheepRepository.GetSome(offset, limit);
    }

    public (List<Cheep>? Cheeps, int AuthorsCheepsCount) GetCheepsByAuthor(string _author, int offset, int limit)
    {
        return _authorRepository.GetAuthorsCheeps(_author, offset, limit);
    }
}