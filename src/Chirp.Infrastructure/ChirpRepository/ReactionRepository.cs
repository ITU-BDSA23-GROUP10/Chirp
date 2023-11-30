using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

public class ReactionRepository : IReactionRepository<Reaction, Cheep, User>
{
    protected DbSet<Reaction> DbSetReaction;
    protected DbSet<Cheep> DbSetCheep;
    protected ChirpDBContext context;

    public UserRepository(ChirpDBContext dbContext)
    {
        DbSetReaction = dbContext.Reaction;
        DbSetCheep = dbContext.Cheep;
        context = dbContext;
    }
    

    #region IReactionRepository<Reaction, Cheep, User> Members

    public void InsertReaction(Reaction entity)
    {
        DbSetReaction.Add(entity);
        context.SaveChanges();
    }

    public void DeleteReaction(Reaction entity)
    {
        DbSetReaction.Remove(entity);
        context.SaveChanges();
    }

    public IQueryable<Reaction> SearchFor(Expression<Func<Reaction, bool>> predicate)
    {
        return DbSetUser.Where(predicate);
    }


    #endregion
}