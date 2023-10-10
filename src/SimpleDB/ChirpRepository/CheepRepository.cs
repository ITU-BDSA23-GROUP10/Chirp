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

    #region IRepository<Cheep> Members

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

    public IQueryable<Cheep> GetAll()
    {
        return DbSet;
    }

    public Cheep GetById(int id)
    {
        return DbSet.Find(id);
    }

    public int GetRowCount()
    {
        return DbSet.Count();
    }

    public IQueryable<Cheep> GetSome(int offset, int limit)
    {
        return DbSet.Skip(offset).Take(limit);
    }

    #endregion
}