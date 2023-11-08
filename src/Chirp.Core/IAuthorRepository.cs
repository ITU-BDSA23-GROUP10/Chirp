using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IAuthorRepository<Author, Cheep>
{
    void Insert(Author entity);
    void Delete(Author entity);
    IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate);
    Task<Author> GetAuthorWithCheeps(string authorName);
    Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit);

    Task<Author?> GetAuthorById(int id);

    Task<Author?> GetAuthorByName(string name);

    Task<Author?> GetAuthorByEmail(string email);

    Task CreateAuthor(string name, string? email = null);
}