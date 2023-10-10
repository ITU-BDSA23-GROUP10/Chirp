using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Models;

namespace SimpleDB.ChirpRepository;

public class CheepRepository : IDatabaseRepository<Cheep>
{
    protected DbSet<Cheep> DbSet;

    public CheepRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Set<Cheep>();
    }

    #region IDatabaseRepository<T> Members

    public void Insert(Cheep entity)
    {
        DbSet.Add(entity);
    }

    public void Delete(Cheep entity)
    {
        DbSet.Remove(entity);
    }

    public IQueryable<Cheep> SearchFor(Expression<Func<Cheep, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public (IQueryable<Cheep>, int) GetAll()
    {
        return (DbSet, DbSet.Count());
    }

    public Cheep? GetById(int id)
    {
        return DbSet.Find(id);
    }

    public (List<Cheep>, int) GetSome(int offset, int limit)
    {
        // From StackOverflow: https://stackoverflow.com/a/29205357
        var page = DbSet.Skip(offset).Take(limit);
        var query = (from p in page
                    select new Cheep
                    {
                        CheepId = p.CheepId,
                        Author = p.Author,
                        Text = p.Text,
                        TimeStamp = p.TimeStamp
                    }).ToList();
        
        return (query, DbSet.Count());
    }

    #endregion
}