using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Chirp.Web.ViewComponents;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep {get; set;} = new();
    
    [BindProperty]
    public NewFollow NewFollow {get; set;} = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;

    // maybe remove
    public List<CheepDTO> UserCheeps { get; set; } = new List<CheepDTO>();

    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public UserTimelineModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep, User> authorService, IUserRepository<User> userService)
    {
        _authorService = authorService;
        _cheepService = cheepService;
        _userService = userService;
    }

    public async Task<IActionResult> OnPost()
    {

        AsyncPadlock padlock = new();
        var userName = User?.Identity?.Name ?? "default";

        try
        {
        await padlock.Lock();
        var author = await _authorService.GetAuthorByName(userName);

        // Create new auther if does not exist in database ready
        if (author is null) 
        {
            await _authorService.CreateAuthor(await _userService.GetUserByName(userName));
            author = await _authorService.GetAuthorByName(userName);
        }

        var cheep = new CheepCreateDTO(NewCheep.Message, userName);
        
        await _cheepService.CreateCheep(cheep, author);

        }
        finally
        {
            padlock.Dispose();
        }

        return Redirect("/" + userName);
    }

    //follow form button
    public async Task<IActionResult> OnPostFollow() 
    {
        var LoggedInUserName = User?.Identity?.Name ?? "default";
        var FollowedUserName = NewFollow.Author;

        if (string.IsNullOrEmpty(FollowedUserName))
        {
            throw new ArgumentException("FollowedUserName cannot be null or empty");
        }
        else
        {
            
            //Check if the user that is logged in exists
            try {
                var loggedInUser = await _userService.GetUserByName(LoggedInUserName);
                if (loggedInUser is null) {
                    throw new Exception("User does not exist");
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                await _userService.CreateUser(LoggedInUserName);
            }

            var followerId = await _userService.GetUserIDByName(LoggedInUserName);
            var followingId = await _userService.GetUserIDByName(FollowedUserName);

            var followDTO = new FollowDTO(followerId, followingId);
            
            await _userService.FollowUser(followDTO);

            return Redirect("/" + LoggedInUserName);
        }
    }

    //unfollow form button
    public async Task<IActionResult> OnPostUnfollow()
    {
        string userName = User?.Identity?.Name ?? "default";

        if (string.IsNullOrEmpty(NewFollow.Author))
        {
            throw new ArgumentException("NewFollow.Author cannot be null or empty");
        }
        else
        {
        // Convert the username to Id
        var followerId = await _userService.GetUserIDByName(userName);
        var followingId = await _userService.GetUserIDByName(NewFollow.Author);

        var unfollowDTO = new FollowDTO(followerId, followingId);
            
        await _userService.UnfollowUser(unfollowDTO);

        return Redirect("/" + userName);
            
        }    
    }

    public async Task<bool> CheckIfFollowed(int userId, int authorId)
    {
        return await _userService.IsFollowing(userId, authorId);
    }

    public async Task<int> FindUserIDByName(string userName)
    {
        return await _userService.GetUserIDByName(userName);
    }

    //get method with pagination
    public async Task<ActionResult> OnGetAsync(string author, [FromQuery(Name = "page")] int page = 1)
    {

        ViewData["Author"] = author;
        ViewData["Page"] = page;

        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();
        try
        {
            await padlock.Lock();
            
            var userName = User?.Identity?.Name ?? "default";
            var userId = await _userService.GetUserIDByName(userName);

            List<int> FollowedUsers = await _userService.GetFollowedUsersId(userId);

            List<CheepDTO> followingCheeps = new List<CheepDTO>();
            int count = 0;

            foreach(int id in FollowedUsers) {
                followingCheeps.AddRange(await _authorService.GetCheepsByAuthorId(id, offset, limit));
                count += await _authorService.GetCheepsCountsFromAuthorId(id);
            }

            if (User?.Identity?.Name == author) // logged-in user's page
            {
                (UserCheeps, int cheepsCount) = await _authorService.GetCheepsByAuthor(author, offset, limit);
                Cheeps.Clear();
                Cheeps.AddRange(UserCheeps);
                Cheeps.AddRange(followingCheeps);
                ViewData["CheepsCount"] = cheepsCount + count;
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