using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Migrations;
using SimpleDB.Models;

namespace SimpleDB.ChirpRepository;

public class AuthorRepository : IDatabaseRepository<Author>
{
    protected DbSet<Author> DbSet;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Set<Author>();
    }

    #region IDatabaseRepository<T> Members

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

    public (List<Cheep>?, int) GetAuthorsCheeps(string author, int offset, int limit)
    {
        // Helge has said we're to assume Author.Name are unique for now.
        var authorEntity = SearchFor(_author => _author.Name == author).FirstOrDefault();

        if (authorEntity == null)
        {
            return (new List<Cheep>(), 0);
        }

        // from StackOverflow: https://stackoverflow.com/a/29205357
        var page = DbSet.Skip(offset).Take(limit);
        var query = (from author_ in page
                    where author_ == authorEntity
                    select author_.Cheeps).FirstOrDefault();

        return (query, DbSet.Count());
    }

    public Author? GetById(int id)
    {
        return DbSet.Find(id);
    }
    #endregion
}