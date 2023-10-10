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

    public List<Cheep> GetCheeps(int offset, int limit) 
    {
        return _cheepRepository.GetSome(offset, limit).ToList();
    }

    public List<Cheep> GetCheepsByAuthor(string _author, int offset, int limit)
    {
        return _authorRepository.GetAuthorsCheeps(_author, offset, limit);
    }
}