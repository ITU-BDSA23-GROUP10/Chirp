using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IAuthorRepository<Author, Cheep>
{
    void Insert(Author entity);
    void Delete(Author entity);
    IQueryable<Author> SearchFor(Expression<Func<Author, bool>> predicate);
    Author GetAuthorWithCheeps(string authorName);
    (List<CheepDTO>, int) GetCheepsByAuthor(string author, int offset, int limit);

    Author? GetAuthorById(int id);

    Author? GetAuthorByName(string name);

    Author? GetAuthorByEmail(string email);

    void CreateAuthor(string name, string email);

    public int GetMaxId();
}