using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class AuthorRepository : IAuthorRepository<Author, Cheep>
{
    protected DbSet<Author> DbSet;
    protected ChirpDBContext context;
    protected int maxid;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Authors;
        context = dbContext;
        maxid = GetMaxId() + 1;
    }

    #region IAuthorRepository<Author, Cheep> Members

    public void Insert(Author entity)
    {
        DbSet.Add(entity);
        context.SaveChanges();
    }

    public void Delete(Author entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public async Task<Author> GetAuthorWithCheeps(string authorName)
    {
        // The returned Author object can use Author.Cheeps to get all the Cheeps (sorted by descending timestamp)
        
        var author = await DbSet.Include(_author => _author.Cheeps
                    .OrderByDescending(_cheep => _cheep.TimeStamp))
                    .Where(_author => _author.Name == authorName)
                    .FirstOrDefaultAsync() ?? throw new Exception($"Author {authorName} not found");
        return author;
    }

    public async Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit)
    {
        // Helge has said we're to assume Author.Name are unique for now.

        int cheepsCount = 0;

        var authorEntity = await GetAuthorByName(author);

        if (authorEntity is null)
        {
            return new Tuple<List<CheepDTO>, int>(new List<CheepDTO>(), cheepsCount);
        }
        else
        {
            cheepsCount = DbSet.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query().Count();
        }

        List<CheepDTO> cheeps = DbSet.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Skip(offset).Take(limit)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.Author.Name,
                        _cheep.Text,
                        _cheep.TimeStamp.ToString()
                    ))
                    .ToList()
                    ?? new List<CheepDTO>();

        return new Tuple<List<CheepDTO>, int>(cheeps, cheepsCount);
    }

    public async Task<Author?> GetAuthorById(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Author?> GetAuthorByName(string name)
    {
        // FirstOrDefault returns null if no Author is found.
        var author = await SearchFor(_author => _author.Name == name).FirstOrDefaultAsync();

        return author;
    }

    public async Task<Author?> GetAuthorByEmail(string email)
    {
        var author = await SearchFor(_author => _author.Email == email).FirstOrDefaultAsync();

        return author;
    }

    public async Task CreateAuthor(string name, string? email)
    {
        Author? author = null;

        if (email is not null)
        {
            author = await GetAuthorByEmail(email);
        }

        if (author is null)
        {
            author = await GetAuthorByName(name);
        }

        if (author is not null)
        {
            throw new Exception("Author already exists");
        }

        if (author is null)
        {
            var authorEnity = new Author()
            {
                Name = name,
                Email = email ?? null,
                Cheeps = new List<Cheep>()
            };
            Insert(authorEnity);

            maxid++;
        }
    }

    public int GetMaxId()
    {
        var query = (from author_ in DbSet
                     select author_.AuthorId)
                    .ToList();

        return query.Max();
    }

    // Author? IAuthorRepository<Author, Cheep>.GetAuthorById(int id)
    // {
    //     throw new NotImplementedException();
    // }
    #endregion
}
