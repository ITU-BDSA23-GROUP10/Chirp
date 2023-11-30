using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

public class ReactionRepository : IReactionRepository<Reaction, Cheep>
{
    protected DbSet<Reaction> DbSetReaction;
    protected DbSet<Cheep> DbSetCheep;
    protected ChirpDBContext context;

    public UserRepository(ChirpDBContext dbContext)
    {
        DbSetUser = dbContext.Users;
        DbSetFollows = dbContext.Follows;
        context = dbContext;
    }

}