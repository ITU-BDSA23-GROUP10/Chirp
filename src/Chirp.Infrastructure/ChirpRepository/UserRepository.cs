using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Migrations;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

public class UserRepository : IUserRepository<User>
{
    protected DbSet<User> DbSet;
    protected ChirpDBContext context;

    public UserRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Users;
        context = dbContext;
    }

    #region IUserRepository<User> Members

    public void Insert(User entity)
    {
        DbSet.Add(entity);
        context.SaveChanges();
    }

    public void Delete(User entity)
    {
        DbSet.Remove(entity);
        context.SaveChanges();
    }

    public IQueryable<User> SearchFor(Expression<Func<User, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await DbSet.FindAsync(id);
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
            Insert(userEntity);
        }
    }
    #endregion
}