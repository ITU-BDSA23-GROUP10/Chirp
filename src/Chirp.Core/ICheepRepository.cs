using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface ICheepRepository<Cheep, Author>
{
    void Insert(Cheep entity);
    void Delete(Cheep entity);
    IQueryable<Cheep> SearchFor(Expression<Func<Cheep, bool>> predicate);
    (IQueryable<Cheep>, int) GetAll();
    Task<Cheep?> GetById(int id);
    Task<Tuple<List<CheepDTO>, int>> GetSome(int offset, int limit);
    Task CreateCheep(CheepCreateDTO cheepCreateDTO, Author author);
}