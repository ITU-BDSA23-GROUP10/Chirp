using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Core;

namespace Chirp.Infrastructure.ChirpRepository;

// The AuthorRepository is used to access the database and perform operations on the Author table. 
// The AuthorRepository implements the IAuthorRepository interface
public class AuthorRepository : IAuthorRepository<Author, Cheep, User>
{
    protected DbSet<Author> DbSetAuthor;
    protected ChirpDBContext context;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        DbSetAuthor = dbContext.Authors;
        context = dbContext;
    }

    #region IAuthorRepository<Author, Cheep> Members
    // The Insert method is used to insert a new Author object into the database.
    private async Task Insert(Author entity)
    {
        DbSetAuthor.Add(entity);
        await context.SaveChangesAsync();
    }
    // The Delete method is used to delete an Author object from the database.
    public async Task Delete(Author entity)
    {
        DbSetAuthor.Remove(entity);
        await context.SaveChangesAsync();
    }
    // The SearchFor method is used to search for an Author object in the database.
    private IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate)
    {
        return DbSetAuthor.Where(predicate);
    }
    // The GetCheepsCountsFromAuthorId method is used to get the number of cheeps from an author using the author's id.
    public async Task<int> GetCheepsCountsFromAuthorId(int id) 
    {
        var authorEntity = await SearchFor(_author => _author.User.UserId == id).FirstOrDefaultAsync();

        // Checks if the author exists
        if (authorEntity == null) {
            return 0;
        }

        int cheepsCount = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .CountAsync();
        return cheepsCount;
    }
    // The GetAuthorByName method is used to get an Author object from the database using the author's name.
    public async Task<Author?> GetAuthorByName(string name)
    {
        // FirstOrDefault returns null if no User is found.
        var author = await SearchFor(_author => _author.User.Name == name).FirstOrDefaultAsync();

        return author;
    }
    // The GetCheepsByAuthor method is used to get a list of CheepDTO objects and the count of cheeps 
    // from the database using the author's name.
    public async Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit)
    {
        int cheepsCount = 0;

        var authorEntity = await SearchFor(_author => _author.User.Name == author).FirstOrDefaultAsync();

        // Checks if the author exists
        if (authorEntity is null)
        {
            return new Tuple<List<CheepDTO>, int>(new List<CheepDTO>(), cheepsCount);
        }
        else
        {   
            cheepsCount = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query().CountAsync();
        }

        List<CheepDTO> cheeps = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Skip(offset).Take(limit)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.CheepId,
                        _cheep.Author.User.Name,
                        _cheep.Text,
                        _cheep.TimeStamp
                    ))
                    .ToListAsync()
                    ?? new List<CheepDTO>();

        return new Tuple<List<CheepDTO>, int>(cheeps, cheepsCount);
    }
    // The GetCheepsByAuthorId method is used to get a list of CheepDTO objects from the database using the author's id.
    public async Task<List<CheepDTO>> GetCheepsByAuthorId(List<int> ids, int offset, int limit)
    {
        var authorEntity = await SearchFor(_author => ids.Contains(_author.User.UserId)).FirstOrDefaultAsync();

        // Checks if the author exists
        if (authorEntity is null)
        {
            return new List<CheepDTO>();
        }

        var cheeps = DbSetAuthor.Where(_author =>  ids.Contains(_author.User.UserId))
                            .SelectMany(_author2 => _author2.Cheeps)
                            .OrderByDescending(_cheep => _cheep.TimeStamp)
                            .Skip(offset)
                            .Take(limit)
                            .Select(_cheep => new CheepDTO
                            (
                                _cheep.CheepId,
                                _cheep.Author.User.Name,
                                _cheep.Text,
                                _cheep.TimeStamp
                            ))
                            .ToList();
                                

        return cheeps;
    }
    // The GetAllCheepsByAuthorName method is used to get a list of CheepDTO objects 
    // from the database using the author's name.

    // NOTE: This method is a little redundant, as you could just extract 
    // the author's cheeps from the author object.
    // However, you would need a method to convert these to cheepDTOs.
    public async Task<List<CheepDTO>> GetAllCheepsByAuthorName(string authorName)
    {
        var authorEntity = await SearchFor(_author => _author.User.Name == authorName).FirstOrDefaultAsync();

        // Checks if the author exists
        if (authorEntity is null)
        {
            return new List<CheepDTO>();
        }

        List<CheepDTO> cheeps = await DbSetAuthor.Entry(authorEntity)
                    .Collection(_author => _author.Cheeps)
                    .Query()
                    .OrderByDescending(_cheep => _cheep.TimeStamp)
                    .Select(_cheep => new CheepDTO
                    (
                        _cheep.CheepId,
                        _cheep.Author.User.Name,
                        _cheep.Text,
                        _cheep.TimeStamp
                    ))
                    .ToListAsync()
                    ?? new List<CheepDTO>();

        return new List<CheepDTO>(cheeps);
    }
    // The GetAuthorById method is used to get an Author object from the database using the author's id.

    // NOTE: This is only used internally in the author repository since we have 
    // swapped over to using Users as our generic "user type" instead of Authors on the front end.
    public async Task<Author?> GetAuthorById(int id)
    {
        return await DbSetAuthor.FindAsync(id);
    }
    // The CreateAuthor method is used to create a new Author object in the database.
    public async Task CreateAuthor(User user)
    {
        // Checks if the user exists
        if (user is null) {
            throw new Exception("User is null");
        }

        Author? author = await GetAuthorById(user.UserId);
        // Checks if the author already exists
        if (author is not null)
        {
            throw new Exception("Author already exists");
        }

        // Create new author if it doesn't exist in database already
        var authorEntity = new Author()
        {
            User = user,
            Cheeps = new List<Cheep>()
        };
        await Insert(authorEntity);
    }
    #endregion
}