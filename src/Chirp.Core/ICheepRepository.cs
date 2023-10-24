using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface ICheepRepository<Cheep, Author>
{
    void Insert(Cheep entity);
    void Delete(Cheep entity);
    IQueryable<Cheep> SearchFor(Expression<Func<Cheep, bool>> predicate);
    (List<CheepDTO>?, int) GetSome(int offset, int limit);
    void CreateCheep(Author author, string text);
}