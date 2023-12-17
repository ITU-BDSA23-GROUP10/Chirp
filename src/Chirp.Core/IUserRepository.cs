using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IUserRepository<User>
{
    Task DeleteUser(User entity);

    Task<User?> GetUserById(int id);

    Task<User?> GetUserByName(string name);

    Task<User?> GetUserByEmail(string email);

    Task<int> GetUserIDByName(string name);

    Task CreateUser(string name, string? email = null);
    Task UpdateUserEmail(string name, string email);
}