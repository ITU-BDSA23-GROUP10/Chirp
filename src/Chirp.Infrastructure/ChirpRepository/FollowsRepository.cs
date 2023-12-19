using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

// The FollowsRepository is used to access the database and perform operations on the Cheep table.
public class FollowsRepository : IFollowsRepository<Follows>
{
    protected DbSet<Follows> DbSetFollows;
    protected ChirpDBContext context;

    public FollowsRepository(ChirpDBContext dbContext)
    {
        DbSetFollows = dbContext.Follows;
        context = dbContext;
    }

    #region IFollowsRepository<User> Members
    // The Insert method is used to insert a new Follows object into the database.
    public async Task InsertFollow(Follows entity)
    {
        DbSetFollows.Add(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete a Follows object from the database.
    public async Task DeleteFollow(Follows entity)
    {
        DbSetFollows.Remove(entity);
        await context.SaveChangesAsync();
    }
    // The FollowUser method is used to follow a user using a followDTO object 
    // The followDTO contains the id of the user following and the id of the user being followed.
    public async Task FollowUser(FollowDTO followDTO)
    {
        var exists = await DbSetFollows.AnyAsync(f => f.FollowerId == followDTO.followerId && f.FollowingId == followDTO.followingId);
        // If the follow record does not exist, create a new one
        if (!exists)
        {
            var newFollow = new Follows()
            {
                FollowerId = followDTO.followerId,
                FollowingId = followDTO.followingId
            };
            await InsertFollow(newFollow);
        }
        else
        {
            throw new Exception("Follow record already exists");
        }
    }
    // The getUserFollowersCountById method is used to get the number of followers 
    // of a user using the user's id.
    public async Task<int> getUserFollowingCountById(int userId)
    {
        return await DbSetFollows.CountAsync(f => f.FollowingId == userId);
    }
    // The IsFollowing method is used to check if a user is following another user.
    public async Task<bool> IsFollowing(int followerId, int followingId)
    {
        return await DbSetFollows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }
    // The UnfollowUser method is used to unfollow a user using a unfollowDTO object
    public async Task UnfollowUser(FollowDTO unfollowDTO)
    {
        var record = await DbSetFollows.FirstOrDefaultAsync(f => f.FollowerId == unfollowDTO.followerId && f.FollowingId == unfollowDTO.followingId);
        
        // If the follow record exists, delete it
        if (record != null)
        {
            await DeleteFollow(record);
        }
        else
        {
            throw new Exception("Unfollow record does not exist");
        }
    }
    // The GetFollowedUsersId method is used to get a list of ids of users followed by a user
    public async Task<List<int>> GetFollowedUsersId(int userId)
    {
        var followedUsers = await DbSetFollows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowingId)
            .ToListAsync();

        return followedUsers;
    }
    // The GetIdsFollowingUser method is used to get a list of ids of users following a user
    public async Task<List<int>> GetIdsFollowingUser(int userId)
    {
        var IdsFollowingUser = await DbSetFollows
            .Where(f => f.FollowingId == userId)
            .Select(f => f.FollowerId)
            .ToListAsync();

        return IdsFollowingUser;
    }

    // The LoopDeleteFollowers method loops through a list of ids and deletes all followers
    public async Task LoopDeleteFollowers(List<int> followedUsers, int userId)
    {
        foreach (int id in followedUsers)
        {
            var follow = new Follows()
            {
                FollowerId = userId,
                FollowingId = id
            };
            await DeleteFollow(follow);
        }
        return;
    }

    // The DeleteAllFollowers method deletes all followers of a user
    // This works reflexively, so it also deletes all follows the user has
    public async Task DeleteAllFollowers(int userId)
    {
        //gets all users the user is following
        List<int> followedUsers = await GetFollowedUsersId(userId);

        //gets all users following the user
        var IdsFollowingUser = await GetIdsFollowingUser(userId);

        //deletes all followers of the user
        await LoopDeleteFollowers(IdsFollowingUser, userId);

        //deletes all follows the user has
        await LoopDeleteFollowers(followedUsers, userId);

        return;
    }
    #endregion
}