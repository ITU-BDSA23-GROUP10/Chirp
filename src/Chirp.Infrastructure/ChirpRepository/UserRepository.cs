using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
//using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class UserRepository : IUserRepository<User>
{
    protected DbSet<User> DbSetUser;
    protected DbSet<Follows> DbSetFollows;
    protected ChirpDBContext context;

    public UserRepository(ChirpDBContext dbContext)
    {
        DbSetUser = dbContext.Users;
        DbSetFollows = dbContext.Follows;
        context = dbContext;
    }

    #region IUserRepository<User> Members

    public void InsertUser(User entity)
    {
        DbSetUser.Add(entity);
        context.SaveChanges();
    }

    public void DeleteUser(User entity)
    {
        DbSetUser.Remove(entity);
        context.SaveChanges();
    }

    public void InsertFollow(Follows entity)
    {
        DbSetFollows.Add(entity);
        context.SaveChanges();
    }

    public void DeleteFollow(Follows entity)
    {
        DbSetFollows.Remove(entity);
        context.SaveChanges();
    }

    public IQueryable<User> SearchFor(Expression<Func<User, bool>> predicate)
    {
        return DbSetUser.Where(predicate);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await DbSetUser.FindAsync(id);
    }

    public async Task<User?> GetUserByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var user = await SearchFor(_user => _user.Name == name).FirstOrDefaultAsync();

        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await SearchFor(_user => _user.Email == email).FirstOrDefaultAsync();

        return user;
    }

    public async Task<int> GetUserIDByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var user = await SearchFor(_user => _user.Name == name).FirstOrDefaultAsync();
        return user?.UserId ?? -1;
    }

    public async Task CreateUser(string name, string? email = null)
    {
        User? user = null;

        if (email is not null)
        {
            user = await GetUserByEmail(email);
        }

        if (user is null)
        {
            user = await GetUserByName(name);
        }

        if (user is not null)
        {
            throw new Exception("User already exists");
        }

        if (user is null)
        {
            var userEntity = new User()
            {
                Name = name,
                Email = email ?? null
            };
            InsertUser(userEntity);
        }
    }
    // checks if user exists and if email is null or empty 
    public async Task UpdateUserEmail(string name, string email)
    {
        var user = await GetUserByName(name);

        // checks if user exists and if email is null or empty
        if (user is null)
        {
            throw new Exception("User does not exist");
        } else if (email is null || email == "")
        {
            throw new Exception("Email is null or empty");
        }

        DbSetUser
        .Where(u => u.UserId == user.UserId)
        .ExecuteUpdate(u => u.SetProperty(e => e.Email, e => email));

        context.SaveChanges();
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
            InsertFollow(newFollow);
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
            DeleteFollow(record);
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
            DeleteFollow(follow);
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