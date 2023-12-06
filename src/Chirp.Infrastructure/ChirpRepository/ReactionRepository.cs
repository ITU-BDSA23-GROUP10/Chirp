using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

namespace Chirp.Infrastructure.ChirpRepository;

public class ReactionRepository : IReactionRepository<Reaction>
{
    protected DbSet<Reaction> DbSetReaction;
    protected ChirpDBContext context;

    public ReactionRepository(ChirpDBContext dbContext)
    {
        DbSetReaction = dbContext.Reactions;
        context = dbContext;
    }
    

    #region IReactionRepository<Reaction> Members

    public async Task InsertReaction(Reaction entity)
    {
        DbSetReaction.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteReaction(Reaction entity)
    {
        DbSetReaction.Remove(entity);
        await context.SaveChangesAsync();
    }

    //https://www.learnentityframeworkcore.com/dbset/modifying-data
    public async Task UpdateReaction(Reaction entity)
    {
        DbSetReaction.Update(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<Reaction> SearchFor(Expression<Func<Reaction, bool>> predicate)
    {
        return DbSetReaction.Where(predicate);
    }

    public async Task<int> GetCheepsUpvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.reactionType.Equals("Upvote"));
    }

    public async Task<int> GetCheepsDownvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.reactionType.Equals("Downvote"));
    }

    public async Task ReactToCheep(ReactionDTO reactionDTO)
    {
        if (!reactionDTO.reactionType.Equals("Upvote") && !reactionDTO.reactionType.Equals("Downvote") && reactionDTO.reactionType is not null)
        {
            throw new ArgumentException("'" + reactionDTO.reactionType + "' is not a valid reactionType");
        }

        // Create new reaction if one does not exist
        var reaction = await GetReactionByUserAndCheep(reactionDTO.userId, reactionDTO.cheepId);
        if (reaction is null)
        {
            var newReaction = new Reaction()
            {
                cheepId = reactionDTO.cheepId,
                userId = reactionDTO.userId,
                reactionType = reactionDTO.reactionType,
            };
            await InsertReaction(newReaction);
            return;
        }
        
        // Delete reaction if reactiontype is the same as existing
        if (reactionDTO.reactionType.Equals(reaction.reactionType))
        {
            await DeleteReaction(reaction);
            return;
        }

        reaction.reactionType = reactionDTO.reactionType;
        
        await UpdateReaction(reaction);
    }

    public async Task<bool> checkUserReacted(int userid, int cheepid)
    {
        if(await DbSetReaction.AnyAsync(r => r.userId == userid && r.cheepId == cheepid))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public async Task<string> checkUserReactionType(int userid, int cheepid)
    {
        if(await DbSetReaction.AnyAsync(r => r.userId == userid 
                                        && r.cheepId == cheepid 
                                        && r.reactionType.Equals("Upvote")))
        {
            return "Upvote";
        }
        else if(await DbSetReaction.AnyAsync(r => r.userId == userid 
                                            && r.cheepId == cheepid 
                                            && r.reactionType.Equals("Downvote")))
        {
            return "Downvote";
        }
        else
        {
            throw new Exception("No Reaction found when check reaction type.  userid: " + userid + " | cheepid: " + cheepid);
        }
    }

    public async Task deleteAllUserReactions(int userid)
    {
        List<Reaction> usersReaction = await GetReactionByUsersId(userid);

        foreach(Reaction react in usersReaction)
        {
            await DeleteReaction(react);
        }
    }

    public async Task<Reaction?> GetReactionByUserAndCheep(int userid, int cheepid)
    {
        var reaction = SearchFor(_react => _react.userId == userid && _react.cheepId == cheepid).FirstOrDefault();
        return reaction;
    }

    public async Task<List<Reaction>> GetReactionByUsersId(int userid)
    {
        var usersReaction = await SearchFor(_react => _react.userId == userid).ToListAsync();

        return usersReaction;
    }
    #endregion
}