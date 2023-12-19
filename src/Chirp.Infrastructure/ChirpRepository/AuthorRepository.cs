using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class AuthorRepository : IAuthorRepository<Author, Cheep, User>
{
    protected DbSet<Author> DbSetAuthor;
    protected ChirpDBContext context;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSetAuthor = dbContext.Authors;
        context = dbContext;
    }

    #region IAuthorRepository<Author, Cheep> Members

    private async Task Insert(Author entity)
    {
        DbSetAuthor.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Author entity)
    {
        DbSetAuthor.Remove(entity);
        await context.SaveChangesAsync();
    }

    private IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSetAuthor.Where(predicate);
    }

    public async Task<Author> GetAuthorWithCheeps(string authorName)
    {
        // The returned Author object can use Author.Cheeps to get all the Cheeps (sorted by descending timestamp)
        
        var author = await DbSetAuthor.Include(_author => _author.Cheeps
                    .OrderByDescending(_cheep => _cheep.TimeStamp))
                    .Where(_author => _author.User.Name == authorName)
                    .Select(_author => new Author
                    {
                        AuthorId = _author.AuthorId,
                        User = _author.User,
                        Cheeps = _author.Cheeps
                    })
                    .FirstOrDefaultAsync() ?? throw new Exception($"Author {authorName} not found");
        return author;
    }

    public async Task<int> GetCheepsCountsFromAuthorId(int id) 
    {
        var authorEntity = await SearchFor(_author => _author.User.UserId == id).FirstOrDefaultAsync();

        if (authorEntity == null) {
            return 0; // make this into an exception
        }

        int cheepsCount = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .CountAsync();
        return cheepsCount;
    }

    public async Task<Author?> GetAuthorByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var author = await SearchFor(_author => _author.User.Name == name).FirstOrDefaultAsync();

        return author;
    }

    //GetCheepsByAuthor should be replaced with GetCheepsByAuthorId, we should search by id not name
    public async Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit)
    {
        int cheepsCount = 0;

        var authorEntity = await SearchFor(_author => _author.User.Name == author).FirstOrDefaultAsync();

        if (authorEntity is null)
        {
            return new Tuple<List<CheepDTO>, int>(new List<CheepDTO>(), cheepsCount);
        }
        else
        {
            cheepsCount = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query().CountAsync();
        }

        List<CheepDTO> cheeps = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Skip(offset).Take(limit)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.CheepId,
                        _cheep.Author.User.Name,
                        _cheep.Text,
                        _cheep.TimeStamp
                    ))
                    .ToListAsync()
                    ?? new List<CheepDTO>();

        return new Tuple<List<CheepDTO>, int>(cheeps, cheepsCount);
    }

    public async Task<List<CheepDTO>> GetCheepsByAuthorId(List<int> ids, int offset, int limit)
    {
        var authorEntity = await SearchFor(_author => ids.Contains(_author.User.UserId)).FirstOrDefaultAsync();

        if (authorEntity is null)
        {
            return new List<CheepDTO>();
        }

        var cheeps = DbSetAuthor.Where(_author =>  ids.Contains(_author.User.UserId))
                            .SelectMany(_author2 => _author2.Cheeps)
                            .OrderByDescending(_cheep => _cheep.TimeStamp)
                            .Skip(offset)
                            .Take(limit)
                            .Select(_cheep => new CheepDTO
                            (
                                _cheep.CheepId,
                                _cheep.Author.User.Name,
                                _cheep.Text,
                                _cheep.TimeStamp
                            ))
                            .ToList();
                                

        return cheeps;
    }

    public async Task<List<CheepDTO>> GetAllCheepsByAuthorName(string authorName)
    {
        var authorEntity = await SearchFor(_author => _author.User.Name == authorName).FirstOrDefaultAsync();

        if (authorEntity is null)
        {
            return new List<CheepDTO>();
        }

        List<CheepDTO> cheeps = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.CheepId,
                        _cheep.Author.User.Name,
                        _cheep.Text,
                        _cheep.TimeStamp
                    ))
                    .ToListAsync()
                    ?? new List<CheepDTO>();

        return new List<CheepDTO>(cheeps);
    }

    public async Task<Author?> GetAuthorById(int id)
    {
        return await DbSetAuthor.FindAsync(id);
    }

    public async Task CreateAuthor(User user)
    {
        if (user is null) {
            throw new Exception("User is null");
        }

        Author? author = await GetAuthorById(user.UserId);
        if (author is not null)
        {
            throw new Exception("Author already exists");
        }

        if (author is null)
        {
            var authorEntity = new Author()
            {
                User = user,
                Cheeps = new List<Cheep>()
            };
            await Insert(authorEntity);
        }
    }


    // Author? IAuthorRepository<Author, Cheep>.GetAuthorById(int id)
    // {
    //     throw new NotImplementedException();
    // }
    #endregion
}