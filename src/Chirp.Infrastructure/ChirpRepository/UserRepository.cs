using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;
using System;

namespace Chirp.Infrastructure.ChirpRepository;

// The UserRepository is used to access the database and perform operations on the User table.
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

    // The Insert method is used to insert a new User object into the database.
    private async Task InsertUser(User entity)
    {
        DbSetUser.Add(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete a User object from the database.
    public async Task DeleteUser(User entity)
    {
        DbSetUser.Remove(entity);
        await context.SaveChangesAsync();
    }
    // The SearchFor method is used to search for a User object in the database.
    private IQueryable<User> SearchFor(Expression<Func<User, bool>> predicate)
    {
        return DbSetUser.Where(predicate);
    }
    // The GetUserById method is used to get a User object from the database using the user's id.
    public async Task<User?> GetUserById(int id)
    {
        return await DbSetUser.FindAsync(id);
    }
    // The GetUserByName method is used to get a User object from the database using the user's name.
    public async Task<User?> GetUserByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var user = await SearchFor(_user => _user.Name == name).FirstOrDefaultAsync();

        return user;
    }
    // The GetUserByEmail method is used to get a User object from the database using the user's email.
    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await SearchFor(_user => _user.Email == email).FirstOrDefaultAsync();

        return user;
    }
    // The GetUserIDByName method is used to get a User's id from the database using the user's name.
    public async Task<int> GetUserIDByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var user = await SearchFor(_user => _user.Name == name).FirstOrDefaultAsync();
        return user?.UserId ?? -1;
    }
    // The CreateUser method is used to create a new User object in the database.
    // The name and email of the user are passed as parameters (email is optional)
    public async Task CreateUser(string name, string? email = null)
    {
        User? user = null;
        // checks if user exists
        if (email is not null)
        {
            user = await GetUserByEmail(email);
        }
        // checks if user exists
        if (user is null)
        {
            user = await GetUserByName(name);
        }
        // If user exists, throw exception
        if (user is not null)
        {
            throw new Exception("User '" + name + "' already exists");
        }
        // If user does not exist, create a new one
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
    // The UpdateUserEmail method is used to update a User's email in the database.
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