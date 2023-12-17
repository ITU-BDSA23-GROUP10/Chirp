using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class UserRepository : IUserRepository<User>
{
    protected DbSet<User> DbSetUser;
    protected ChirpDBContext context;

    public UserRepository(ChirpDBContext dbContext)
    {
        DbSetUser = dbContext.Users;
        context = dbContext;
    }

    #region IUserRepository<User> Members

    public async Task InsertUser(User entity)
    {
        DbSetUser.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(User entity)
    {
        DbSetUser.Remove(entity);
        await context.SaveChangesAsync();
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
            throw new Exception("User '" + name + "' already exists");
        }

        if (user is null)
        {
            var userEntity = new User()
            {
                Name = name,
                Email = email ?? null
            };
            await InsertUser(userEntity);
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
    #endregion
}