using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;
using System.Linq;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure;

namespace SimpleDB;

public class DBFacade 
{
    private readonly CheepRepository _cheepRepository;
    private readonly AuthorRepository _authorRepository;

    public DBFacade(ChirpDBContext dbContext)
    {   
        _authorRepository = new AuthorRepository(dbContext);
        _cheepRepository = new CheepRepository(dbContext);
    }

    public (List<CheepDTO> Cheeps, int CheepsCount) GetCheeps(int offset, int limit) 
    {
        return _cheepRepository.GetSome(offset, limit);
    }

    public (List<CheepDTO>? Cheeps, int AuthorsCheepsCount) GetCheepsByAuthor(string _author, int offset, int limit)
    {
        return _authorRepository.GetAuthorsCheeps(_author, offset, limit);
    }
}