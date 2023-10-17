using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

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
        // Order by desc (x => x.Field) from StackOverflow: https://stackoverflow.com/a/5813479
        var query = (from cheep in DbSet
                    .OrderByDescending(d => d.TimeStamp)
                    .Skip(offset)
                    .Take(limit)
                    select new Cheep
                    {
                        CheepId = cheep.CheepId,
                        Author = cheep.Author,
                        Text = cheep.Text,
                        TimeStamp = cheep.TimeStamp
                    })
                    .ToList();

        return (query, DbSet.Count());
    }

    #endregion
}