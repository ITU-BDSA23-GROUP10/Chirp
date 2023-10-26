using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

public class AuthorRepository : IAuthorRepository<Author, Cheep>
{
    protected DbSet<Author> DbSet;
    protected int maxid;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Set<Author>();
        maxid = GetMaxId() + 1;
    }

    #region IAuthorRepository<Author, Cheep> Members

    public void Insert(Author entity)
    {
        DbSet.Add(entity);
    }

    public void Delete(Author entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public (List<CheepDTO>, int) GetCheepsByAuthor(string author, int offset, int limit)
    {
        // Helge has said we're to assume Author.Name are unique for now.
        var authorEntity = SearchFor(_author => _author.Name == author).FirstOrDefault();

        Console.WriteLine($"Author's cheepcount: {authorEntity?.Cheeps.Count()}");

        if (authorEntity is null)
        {
            return (new List<CheepDTO>(), 0);
        }

        // query format from StackOverflow: https://stackoverflow.com/a/29205357
        // from from stm from StackOverflow: https://stackoverflow.com/a/6257269
        // orderby descending inspired from StackOverflow: https://stackoverflow.com/a/9687214
        IQueryable<Cheep>? query = (from author_ in DbSet
                     where author_ == authorEntity
                     from cheep in author_.Cheeps
                     orderby cheep.TimeStamp descending
                     select cheep);

        if (query is null)
        {
            return (new List<CheepDTO>(), 0);
        }

        List<CheepDTO> cheeps = new List<CheepDTO>();
        //foreach (Cheep cheep in query.Skip(offset).Take(limit).ToList())
        foreach (Cheep cheep in authorEntity.Cheeps.Skip(offset).Take(limit))
        {
            cheeps.Add(new CheepDTO
            (
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp.ToString()
            ));
        }

        return (cheeps, query.Count());
    }

    public Author? GetAuthorById(int id)
    {
        return DbSet.Find(id);
    }

    public Author? GetAuthorByName(string name)
    {
        // FirstOrDefault returns null if no Author is found.
        var author = SearchFor(_author => _author.Name == name).FirstOrDefault();

        return author;
    }

    public Author? GetAuthorByEmail(string email)
    {
        var author = SearchFor(_author => _author.Email == email).FirstOrDefault();

        return author;
    }

    public void CreateAuthor(string name, string email)
    {
        Author? author = null;
        author = GetAuthorByEmail(email);
        if (author is null)
        {
            author = GetAuthorByName(name);
        }

        if (author is not null)
        {
            throw new Exception("Author already exists");
        }

        if (author is null)
        {
            Insert(new Author()
            {
                AuthorId = maxid,
                Name = name,
                Email = email,
                Cheeps = new List<Cheep>()
            });
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
    #endregion
}
