using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

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

        return (query, authorEntity.Cheeps.Count);
    }

    public Author? GetById(int id)
    {
        return DbSet.Find(id);
    }
    #endregion
}