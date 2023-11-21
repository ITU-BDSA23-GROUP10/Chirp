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

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSet = dbContext.Authors;
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

    public IQueryable<Author> SearchFor(Expression<Func<User, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public async Task<Author?> GetUserById(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Author?> GetUserByName(string name)
    {
        // FirstOrDefault returns null if no Author is found.
        var author = await SearchFor(_author => _author.Name == name).FirstOrDefaultAsync();

        return author;
    }

    public async Task<Author?> GetUserByEmail(string email)
    {
        var author = await SearchFor(_author => _author.Email == email).FirstOrDefaultAsync();

        return author;
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
                Email = email ?? null,
                Cheeps = new List<Cheep>()
            };
            Insert(userEntity);
        }
    }
    #endregion
}