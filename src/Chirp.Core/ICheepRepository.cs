namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

// The ICheepRepository is used to abstract the database from the rest of the application.
public interface ICheepRepository<Cheep, Author>
{
    Task Delete(int cheepId);
    (IQueryable<Cheep>, int) GetAll();
    Task<Cheep?> GetById(int id);
    Task<Tuple<List<CheepDTO>, int>> GetSome(int offset, int limit);
    Task CreateCheep(CheepCreateDTO cheepCreateDTO, Author author);
    Task<List<CheepDTO>> GetCheepsByHashtag(string hashtag);
}