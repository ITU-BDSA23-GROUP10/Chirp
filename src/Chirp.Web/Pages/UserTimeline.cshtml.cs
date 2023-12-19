using Chirp.Core;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel : BasePageModel
{
    public UserTimelineModel(
        ICheepRepository<Cheep, Author> cheepService,
        IAuthorRepository<Author, Cheep, User> authorService,
        IUserRepository<User> userService,
        IReactionRepository<Reaction> reactionService,
        IFollowsRepository<Follows> followsService)
        : base(cheepService, authorService, userService, reactionService, followsService)
        {
        }
    //get method with pagination
    public async Task<ActionResult> OnGetAsync(string author, [FromQuery(Name = "page")] int page = 1)
    {
        var userName = User?.Identity?.Name ?? "default";
        var userId = await _userService.GetUserIDByName(author);

        ViewData["Author"] = author;
        ViewData["Page"] = page;

        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();
        try
        {
            await padlock.Lock();

            if(userId != -1) 
            {
                ViewData["UserExists"] = "true";
            }
            else 
            {   
                ViewData["UserExists"] = "false";
            }
            
            List<int> FollowedUsers = await _followsService.GetFollowedUsersId(userId);
            FollowedUsers.Add(userId);

            List<CheepDTO> followingCheeps = new List<CheepDTO>();
            int followedUsersCheepsCount = 0;

            foreach(int id in FollowedUsers) {
                followedUsersCheepsCount += await _authorService.GetCheepsCountsFromAuthorId(id);
            }
            followingCheeps.AddRange(await _authorService.GetCheepsByAuthorId(FollowedUsers, offset, limit));

            if (userName == author) // logged-in user's page
            {
                Cheeps.Clear();
                Cheeps.AddRange(followingCheeps);
                ViewData["CheepsCount"] = followedUsersCheepsCount;
            }
            else // other users' pages
            {
                (UserCheeps, int cheepsCount) = await _authorService.GetCheepsByAuthor(author, offset, limit);
                Cheeps.Clear();
                Cheeps.AddRange(UserCheeps);
                ViewData["CheepsCount"] = cheepsCount;
            }
            Cheeps = Cheeps.OrderByDescending(c => c.Timestamp).ToList();    
        }
        finally
        {
            padlock.Dispose();
        }

        return Page();
    }
}