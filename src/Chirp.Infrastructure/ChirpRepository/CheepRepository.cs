using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

public class CheepRepository : ICheepRepository<Cheep, Author>
{
    protected DbSet<Cheep> DbSet;
    protected int maxid;

    public CheepRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Set<Cheep>();
        maxid = GetMaxId() + 1;
    }

    #region ICheepRepository<Cheep, Author> Members

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

    public async Task<Cheep?> GetById(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Tuple<List<CheepDTO>, int>> GetSome(int offset, int limit)
    {
        // From StackOverflow: https://stackoverflow.com/a/29205357
        // Order by desc (x => x.Field) from StackOverflow: https://stackoverflow.com/a/5813479
        var query = await (from cheep in DbSet
                    .OrderByDescending(d => d.TimeStamp)
                    .Skip(offset)
                    .Take(limit)
                     select new CheepDTO
                     (
                         cheep.Author.Name,
                         cheep.Text,
                         cheep.TimeStamp.ToString()
                     ))
                    .ToListAsync();

        return new Tuple<List<CheepDTO>, int>(query, DbSet.Count());
    }

    public void CreateCheep(Author? author, string text)
    {
        // Before running CreateCheep from CheepService you must make sure to first run CreateAuthor from Author repo
        // To ensure that the author is either created or already exists!!!
        // THIS SHOULD NOT BE DONE FROM THE CHEEP REPO AS THIS IS NOT ITS CONCERN!

        if (author is null) 
        {
            // This should most likely be changed to a custom exception pertaining to accounts not existing
            throw new Exception("Author doesn't exist try again after creating an account");
        }

        // For future consideration: DateTime.UTCNow vs .Now from StackOverflow: https://stackoverflow.com/questions/62151/datetime-now-vs-datetime-utcnow
        DateTime timestamp = DateTime.Now;

        Insert(new Cheep()
        {
            CheepId = maxid,
            Author = author,
            Text = text,
            TimeStamp = timestamp
        });
        maxid++;
    }
    
    public int GetMaxId()
    {
        var query = (from cheep in DbSet
                     select cheep.CheepId)
                    .ToList();

        return query.Max();
    }
    #endregion
}