using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

namespace Chirp.Infrastructure.ChirpRepository;

// The CheepRepository is used to access the database and perform operations on the Cheep table.
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
    // The Insert method is used to insert a new Cheep object into the database.
    private async Task Insert(Cheep entity)
    {
        DbSet.Add(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete a Cheep object from the database (using a Cheep object).
    private async Task Delete(Cheep entity)
    {
        DbSet.Remove(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete a Cheep object from the database (using a Cheep id).
    public async Task Delete(int cheepId)
    {
        await Delete(GetById(cheepId).Result!);
    }
    // The SearchFor method is used to search for a Cheep object in the database.
    private IQueryable<Cheep> SearchFor(Expression<Func<Cheep, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }
    // The GetAll method is used to get all Cheep objects from the database and the number of Cheeps.
    public (IQueryable<Cheep>, int) GetAll()
    {
        return (DbSet, DbSet.Count());
    }
    // The GetById method is used to get a Cheep object from the database using the Cheep's id.
    public async Task<Cheep?> GetById(int id)
    {
        return await DbSet.FindAsync(id);
    }
    // The GetSome method is used to get a number of Cheep objects from the database.
    // The method returns a tuple containing a list of CheepDTO objects and the number of Cheeps.
    // The method uses an offset and a limit to get the Cheeps as to limit the number of Cheeps returned.
    // Which limits the size of the query and the amount of data sent over the "network".
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
    // The CreateCheep method is used to create a new Cheep object in the database.

    // NOTE:Before running CreateCheep from CheepService you must make sure to first run 
    // CreateAuthor from Author repo to ensure that the author is either created or already exists! 
    public async Task CreateCheep(CheepCreateDTO newCheep, Author author)
    {
        // Checks if the author exists
        if (author is null) 
        {
            throw new Exception("Author doesn't exist try again after creating an account");
        }

        // Checks if the cheep is valid using Fluent Validator
        // https://docs.fluentvalidation.net/en/latest/
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
            throw new Exception(e.Message);
        }
    }
    // The GetCheepsByHashtag method is used to get a list of CheepDTO objects 
    // from the database using a hashtag.
    public async Task<List<CheepDTO>> GetCheepsByHashtag(string hashtag)
    {
        var cheeps = await (
            from cheep in context.Cheeps
            .OrderByDescending(d => d.TimeStamp)
            where cheep.Text.Contains("#" + hashtag)
            select new CheepDTO
            (
                cheep.CheepId,
                cheep.Author.User.Name,
                cheep.Text,
                cheep.TimeStamp
            ))
            .ToListAsync();

        return cheeps;
    }
    
    #endregion
}

// The CheepCreateValidator is used to validate a Cheep.
// It checks if the author is not null and if the text is not empty and is less than 160 characters.
public class CheepCreateValidator : AbstractValidator<CheepCreateDTO> 
{
    public CheepCreateValidator()
    {
        RuleFor(x => x.author).NotNull();
        RuleFor(x => x.text).NotEmpty().Length(0, 160);
    }
}