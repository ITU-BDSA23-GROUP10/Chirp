using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IAuthorRepository<Author, Cheep>
{
    void Insert(Author entity);
    void Delete(Author entity);
    IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate);

    (List<CheepDTO>?, int) GetAuthorsCheeps(string author, int offset, int limit);

    Author? GetAuthorByName(string name);

    Author? GetAuthorByEmail(string email);

    void CreateAuthor(string name, string email);
}