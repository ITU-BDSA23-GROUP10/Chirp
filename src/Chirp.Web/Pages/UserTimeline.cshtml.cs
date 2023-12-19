using Chirp.Core;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

// This is the page model for the UserTimeline page (the user timeline page)
// This page is used to display all cheeps from the user that is logged in as well as all the cheeps from users they are following
// There is also a cheep creation form on this page as well as the ability to follow authors and the ability to react to cheeps
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
    // This method is used to get and display the cheeps on the page as well as some code for pagination
    public async Task<ActionResult> OnGetAsync(string author, [FromQuery(Name = "page")] int page = 1)
    {
        var userName = User?.Identity?.Name ?? "default";
        var userId = await _userService.GetUserIDByName(author);

        ViewData["Author"] = author;
        ViewData["Page"] = page;

        // Pagination
        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();
        try
        {
            await padlock.Lock();

            // This is used to check if the user exists
            if(userId != -1) 
            {
                ViewData["UserExists"] = "true";
            }
            else 
            {   
                ViewData["UserExists"] = "false";
            }
            
            // Get the followed user ids and also add the logged-in user's id to the list
            List<int> FollowedUsers = await _followsService.GetFollowedUsersId(userId);
            FollowedUsers.Add(userId);

            List<CheepDTO> followingCheeps = new List<CheepDTO>();
            int followedUsersCheepsCount = 0;

            // This foreach counts the cheeps from the users that are being followed
            foreach(int id in FollowedUsers) {
                followedUsersCheepsCount += await _authorService.GetCheepsCountsFromAuthorId(id);
            }

            // This adds all the cheeps from the users that are being followed to the list as well as the logged-in user's cheeps
            // This is done by using the list of ids from before
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