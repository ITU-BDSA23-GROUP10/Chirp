using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class AuthorRepository : IAuthorRepository<Author, Cheep, User>
{
    protected DbSet<Author> DbSetAuthor;
    protected DbSet<User> DbSetUser;
    protected ChirpDBContext context;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSetAuthor = dbContext.Authors;
        context = dbContext;
    }

    #region IAuthorRepository<Author, Cheep> Members

    public void Insert(Author entity)
    {
        DbSetAuthor.Add(entity);
        context.SaveChanges();
    }

    public void Delete(Author entity)
    {
        DbSetAuthor.Remove(entity);
        context.SaveChanges();
    }

    public IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSetAuthor.Where(predicate);
    }

    public async Task<Author> GetAuthorWithCheeps(string authorName)
    {
        // The returned Author object can use Author.Cheeps to get all the Cheeps (sorted by descending timestamp)
        
        var author = await DbSetAuthor.Include(_author => _author.Cheeps
                    .OrderByDescending(_cheep => _cheep.TimeStamp))
                    .Where(_author => _author.User.Name == authorName)
                    .FirstOrDefaultAsync() ?? throw new Exception($"Author {authorName} not found");
        return author;
    }

    public async Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit)
    {
        // Helge has said we're to assume Author.Name are unique for now.

        int cheepsCount = 0;

        var authorEntity = SearchFor(_author => _author.User.Name == author).FirstOrDefault();

        if (authorEntity is null)
        {
            return new Tuple<List<CheepDTO>, int>(new List<CheepDTO>(), cheepsCount);
        }
        else
        {
            cheepsCount = DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query().Count();
        }

        List<CheepDTO> cheeps = DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Skip(offset).Take(limit)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.Author.User.Name,
                        _cheep.Text,
                        _cheep.TimeStamp.ToString()
                    ))
                    .ToList()
                    ?? new List<CheepDTO>();

        return new Tuple<List<CheepDTO>, int>(cheeps, cheepsCount);
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
            Insert(authorEntity);
        }
    }


    // Author? IAuthorRepository<Author, Cheep>.GetAuthorById(int id)
    // {
    //     throw new NotImplementedException();
    // }
    #endregion
}