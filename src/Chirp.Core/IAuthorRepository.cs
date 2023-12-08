using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IAuthorRepository<Author, Cheep, User>
{
    Task Insert(Author entity);
    Task Delete(Author entity);
    Task Reload(Author entity);
    Task Reload(int authorId);
    IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate);
    Task<Author> GetAuthorWithCheeps(string authorName);
    Task<int> GetCheepsCountsFromAuthorId(int id);
    Task<Author?> GetAuthorByName(string name);
    Task<Tuple<List<CheepDTO>, int>> GetCheepsByAuthor(string author, int offset, int limit);
    Task<List<CheepDTO>> GetCheepsByAuthorId(int id, int offset, int limit);
    Task<List<CheepDTO>> GetAllCheepsByAuthorName(string authorName);
    Task CreateAuthor(User user);
}