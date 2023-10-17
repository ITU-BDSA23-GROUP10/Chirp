using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

public class AuthorRepository : IDatabaseRepository<Author>
{
    protected DbSet<Author> DbSet;
    protected ChirpDBContext _dbContext;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Set<Author>();
    }

    #region IDatabaseRepository<T> Members

    public void Insert(Author entity)
    {
        DbSet.Add(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(Author entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public (List<CheepDTO>?, int) GetAuthorsCheeps(string author, int offset, int limit)
    {
        // Helge has said we're to assume Author.Name are unique for now.
        var authorEntity = SearchFor(_author => _author.Name == author).FirstOrDefault();

        if (authorEntity == null)
        {
            return (null, 0);
        }

        // query format from StackOverflow: https://stackoverflow.com/a/29205357
        // from from stm from StackOverflow: https://stackoverflow.com/a/6257269
        // orderby descending inspired from StackOverflow: https://stackoverflow.com/a/9687214
        var query = (from author_ in DbSet
                    where author_ == authorEntity
                    from cheep in author_.Cheeps
                    orderby cheep.TimeStamp descending
                    select cheep)
                    .Skip(offset)
                    .Take(limit)
                    .ToList();

        List<CheepDTO> cheeps = new List<CheepDTO>();
        foreach(Cheep cheep in query)
        {
            cheeps.Add(new CheepDTO 
            (
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp.ToString()
            ));
        }

        return (cheeps, authorEntity.Cheeps.Count);
    }

    public Author? GetById(int id)
    {
        return DbSet.Find(id);
    }

    public Author? GetAuthorByName(string name) 
    {
        //Not sure if author returns null if nothing is found, it probably should do that though
        var author = SearchFor(_author => _author.Name == name).FirstOrDefault();

        return author;
    }

    public Author? GetAuthorByEmail(string email)
    {
        //Not sure if author returns null if nothing is found, it probably should do that though
        var author = SearchFor(_author => _author.Email == email).FirstOrDefault(); 

        return author;
    }

    public void CreateAuthor(string name, string email) 
    {
        Author author = null;
        author = GetAuthorByEmail(email); 
        if(author == null) 
        {
            author = GetAuthorByName(name);
        }
 
        if(author == null) 
        {
        Insert(new Author()
        { 
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        });
        }  
    }
    #endregion
}
