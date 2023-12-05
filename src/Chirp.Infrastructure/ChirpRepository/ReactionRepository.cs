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

    //https://www.learnentityframeworkcore.com/dbset/modifying-data
    public void UpdateReaction(Reaction entity)
    {
        DbSetReaction.Update(entity);
        context.SaveChanges();
    }

    public IQueryable<Reaction> SearchFor(Expression<Func<Reaction, bool>> predicate)
    {
        return DbSetReaction.Where(predicate);
    }

    public async Task<int> GetCheepsUpvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.upVote == true);
    }

    public async Task<int> GetCheepsDownvoteCountsFromCheepID(int id) 
    {
        return await DbSetReaction.CountAsync(r => r.cheepId == id && r.downVote == true);
    }

    public async Task ReactToCheep(ReactionDTO reactionDTO)
    {
        //throw new Exception("|| in the reactToCheep ||");
        var exists = await checkUserReacted(reactionDTO.userId, reactionDTO.cheepId);

        if (!exists)
        {
            var reaction = new Reaction()
            {
                cheepId = reactionDTO.cheepId,
                userId = reactionDTO.userId,
            };
            if(reactionDTO.reactionType.Equals("Upvote"))
            {
                reaction.upVote = true;
            }
            else if(reactionDTO.reactionType.Equals("Downvote"))
            {
                reaction.downVote = true;
            }
            else 
            {
                throw new Exception("Problem with creating the reaction. cheepid: " + reactionDTO.cheepId +  " | userId: " + reactionDTO.userId + " | reactionType: " + reactionDTO.reactionType);
            }

            InsertReaction(reaction);
        }
        else
        {
            var reaction = new Reaction()
            {
                cheepId = reactionDTO.cheepId,
                userId = reactionDTO.userId,
            };

            string reactionType = await checkUserReactionType(reaction.userId, reaction.cheepId);
            if(reactionType.Equals("Upvote"))
            {
                if(reactionDTO.reactionType.Equals("Upvote")) 
                {
                    reaction.upVote = true;
                    DeleteReaction(reaction);
                    return;  
                }
                else
                {
                    reaction.upVote = false;
                    reaction.downVote = true;
                }

            }
            else if(reactionType.Equals("Downvote"))
            {
                if(reactionDTO.reactionType.Equals("Downvote")) 
                {
                    reaction.downVote = true;
                    DeleteReaction(reaction); 
                    return;   
                }
                else
                {
                    reaction.upVote = true;
                    reaction.downVote = false;
                }
                
            }
            else 
            {
                throw new Exception("No Reaction Type found when check what type it was");
            }   
            UpdateReaction(reaction);
        }
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
                                        && r.upVote == true))
        {
            return "Upvote";
        }
        else if(await DbSetReaction.AnyAsync(r => r.userId == userid 
                                            && r.cheepId == cheepid 
                                            && r.downVote == true))
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
            DeleteReaction(react);
        }
    }

    public async Task<List<Reaction>> GetReactionByUsersId(int userid)
    {
        var usersReaction = await SearchFor(_react => _react.userId == userid).ToListAsync();

        return usersReaction;
    }


    #endregion
}