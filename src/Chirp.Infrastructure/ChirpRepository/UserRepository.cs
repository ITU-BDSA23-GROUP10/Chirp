using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
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

    public async Task FollowUser(FollowDTO followDTO)
    {
        var newFollow = new Follows()
        {
            FollowerId = followDTO.followerId,
            FollowingId = followDTO.followingId
        };
        InsertFollow(newFollow);
        
    }

    #endregion
}