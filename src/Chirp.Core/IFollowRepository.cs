using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IFollowsRepository<Follows>
{   
    Task FollowUser(FollowDTO followDTO);
    Task<bool> IsFollowing(int followerId, int followingId);
    Task UnfollowUser(FollowDTO unfollowDTO);
    Task<List<int>> GetFollowedUsersId(int userId);
    Task<List<int>> GetIdsFollowingUser(int userId);
    Task LoopDeleteFollowers(List<int> followedUsers, int userId);
    Task DeleteAllFollowers(int userId);
}