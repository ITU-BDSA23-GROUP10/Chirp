using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

namespace Chirp.Infrastructure.ChirpRepository;

public class CheepRepository : ICheepRepository<Cheep, Author>
{
    protected DbSet<Cheep> DbSet;
    protected ChirpDBContext context;
    protected CheepCreateValidator validator = new CheepCreateValidator();  

    public CheepRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Cheeps;
        context = dbContext;
    }

    #region ICheepRepository<Cheep, Author> Members

    public async Task Insert(Cheep entity)
    {
        DbSet.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Cheep entity)
    {
        DbSet.Remove(entity);
        await context.SaveChangesAsync();
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
                        cheep.CheepId,
                        cheep.Author.User.Name,
                        cheep.Text,
                        cheep.TimeStamp
                     ))
                    .ToListAsync();

        return new Tuple<List<CheepDTO>, int>(query, DbSet.Count());
    }

    public async Task CreateCheep(CheepCreateDTO newCheep, Author author)
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
        try
        {
            var validationResult = validator.Validate(newCheep);
            bool status = validationResult.IsValid;

            if(!status)
            {
                List<ValidationFailure> failures = validationResult.Errors;
                throw new Exception(string.Join(", ", failures));
            }

            DateTime timestamp = DateTime.Now;
            await Insert(new Cheep()
            {
                Author = author,
                Text = newCheep.text,
                TimeStamp = timestamp
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, ", failed validation");
            throw e;
        }
    }
    //hashtags
    public async Task<List<CheepDTO>> GetCheepsByHashtag(string hashtag)
    {
        var cheeps = await (
            from cheep in context.Cheeps
            .OrderByDescending(d => d.TimeStamp)
            where cheep.Text.Contains("#" + hashtag)
            select new CheepDTO
            (
                cheep.Author.User.Name,
                cheep.Text,
                cheep.TimeStamp
            ))
            .ToListAsync();

        return cheeps;
    }
    
    #endregion
}

public class CheepCreateValidator : AbstractValidator<CheepCreateDTO> 
{
    public CheepCreateValidator()
    {
        RuleFor(x => x.author).NotNull();
        RuleFor(x => x.text).NotEmpty().Length(0, 160);
    }
}