using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using FluentValidation;
using FluentValidation.Results;

namespace Chirp.Infrastructure.ChirpRepository;

// The ReactionRepository is used to access the database and perform operations on the Reaction table.
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

    // The Insert method is used to insert a new Reaction object into the database.
    private async Task InsertReaction(Reaction entity)
    {
        DbSetReaction.Add(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete a Reaction object from the database.
    private async Task DeleteReaction(Reaction entity)
    {
        DbSetReaction.Remove(entity);
        await context.SaveChangesAsync();
    }

    // The Update method is used to update a Reaction object from the database.
    // https://www.learnentityframeworkcore.com/dbset/modifying-data
    public async Task UpdateReaction(Reaction entity)
    {
        DbSetReaction.Update(entity);
        await context.SaveChangesAsync();
    }
    // The SearchFor method is used to search for a Reaction object in the database.
    private IQueryable<Reaction> SearchFor(Expression<Func<Reaction, bool>> predicate)
    {
        return DbSetReaction.Where(predicate);
    }
    // The GetCheepsUpvoteCountsFromCheepID method is used to get the number of upvotes
    public async Task<int> GetCheepsUpvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.reactionType.Equals("Upvote"));
    }
    // The GetCheepsDownvoteCountsFromCheepID method is used to get the number of downvotes
    public async Task<int> GetCheepsDownvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.reactionType.Equals("Downvote"));
    }
    // The ReactToCheep method is used to react to a cheep using a reactionDTO object
    public async Task ReactToCheep(ReactionDTO reactionDTO)
    {   
        // throw exception if reactionType is not valid
        if (!reactionDTO.reactionType.Equals("Upvote") && !reactionDTO.reactionType.Equals("Downvote") && reactionDTO.reactionType is not null)
        {
            throw new ArgumentException("'" + reactionDTO.reactionType + "' is not a valid reactionType");
        }

        // Create new reaction if one does not exist
        var reaction = await GetReactionByUserAndCheep(reactionDTO.userId, reactionDTO.cheepId);
        // If the reaction record does not exist, create a new one
        if (reaction is null)
        {
            var newReaction = new Reaction()
            {
                cheepId = reactionDTO.cheepId,
                userId = reactionDTO.userId,
                reactionType = reactionDTO.reactionType!
            };
            await InsertReaction(newReaction);
            return;
        }
        
        // Delete reaction if reactiontype is the same as existing
        if (reactionDTO.reactionType!.Equals(reaction.reactionType))
        {
            await DeleteReaction(reaction);
            return;
        }

        reaction.reactionType = reactionDTO.reactionType;
        
        await UpdateReaction(reaction);
    }
    // The checkUserReacted method is used to check if a user has reacted to a cheep
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
    // The checkUserReactionType method is used to check the type of reaction a user has to a cheep
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
    // The deleteAllUserReactions method is used to delete all reactions of a user
    public async Task deleteAllUserReactions(int userid)
    {
        List<Reaction> usersReaction = await GetReactionByUsersId(userid);

        foreach(Reaction react in usersReaction)
        {
            await DeleteReaction(react);
        }
    }
    // The GetReactionByUserAndCheep method is used to get a reaction of a user to a cheep
    public async Task<Reaction?> GetReactionByUserAndCheep(int userid, int cheepid)
    {
        var reaction = await SearchFor(_react => _react.userId == userid && _react.cheepId == cheepid).FirstOrDefaultAsync();
        return reaction;
    }
    // The GetReactionByUsersId method is used to get all reactions of a user
    public async Task<List<Reaction>> GetReactionByUsersId(int userid)
    {   
        var usersReaction = await SearchFor(_react => _react.userId == userid).ToListAsync();

        return usersReaction;
    }
    #endregion
}