using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

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

    public async Task InsertFollow(Follows entity)
    {
        DbSetFollows.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFollow(Follows entity)
    {
        DbSetFollows.Remove(entity);
        await context.SaveChangesAsync();
    }
    public async Task FollowUser(FollowDTO followDTO)
    {
        var exists = await DbSetFollows.AnyAsync(f => f.FollowerId == followDTO.followerId && f.FollowingId == followDTO.followingId);

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

    public async Task<int> getUserFollowingCountById(int userId)
    {
        return await DbSetFollows.CountAsync(f => f.FollowingId == userId);
    }

    //is the author following or not?
    public async Task<bool> IsFollowing(int followerId, int followingId)
    {
        return await DbSetFollows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    //unfollowing an author
    public async Task UnfollowUser(FollowDTO unfollowDTO)
    {
        var record = await DbSetFollows.FirstOrDefaultAsync(f => f.FollowerId == unfollowDTO.followerId && f.FollowingId == unfollowDTO.followingId);

        if (record != null)
        {
            await DeleteFollow(record);
        }
        else
        {
            throw new Exception("Unfollow record does not exist");
        }
    }

    //Returns all ids of users a user is following
    public async Task<List<int>> GetFollowedUsersId(int userId)
    {
        var followedUsers = await DbSetFollows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowingId)
            .ToListAsync();

        return followedUsers;
    }

    //Returns all ids of users following a user
    public async Task<List<int>> GetIdsFollowingUser(int userId)
    {
        var IdsFollowingUser = await DbSetFollows
            .Where(f => f.FollowingId == userId)
            .Select(f => f.FollowerId)
            .ToListAsync();

        return IdsFollowingUser;
    }

    //deletes all followers of a user
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