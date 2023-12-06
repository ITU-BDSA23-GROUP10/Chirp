using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Chirp.Web.ViewComponents;
using System.Text.RegularExpressions;
using System.Text;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep { get; set; } = new NewCheep { Message = string.Empty};
    
    [BindProperty]
    public NewFollow NewFollow { get; set; } = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;

    //TODO: Figure out why 2 extra pages are added to the pagination
    private readonly int excessiveCheepsCount = 32*2;

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

            // Create new author if it doesn't exist in database allready
            if (author is null) 
            {
                var user = await _userService.GetUserByName(userName);

                if (user is null) {
                    await _userService.CreateUser(userName);
                    user = await _userService.GetUserByName(userName)
                        ?? throw new InvalidOperationException("author could not be created.");
                }

                await _authorService.CreateAuthor(user);
                author = await _authorService.GetAuthorByName(userName);
            }

            if (author is null) 
            {
                throw new InvalidOperationException("author could not be created.");
            }

        if(NewCheep.Message is null || NewCheep.Message.Length < 1)
        {
            ViewData["CheepTooShort"] = "true";
            return Page();
        }
        else 
        {
            ViewData["CheepTooShort"] = "false";
            
            var cheep = new CheepCreateDTO(NewCheep.Message, userName);
            await _cheepService.CreateCheep(cheep, author);
        }

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
            var userId = await _userService.GetUserIDByName(author);
            
            if(userId != -1) 
            {
                ViewData["UserExists"] = "true";
            } else 
            {   
                ViewData["UserExists"] = "false";
            }
            
            List<int> FollowedUsers = await _userService.GetFollowedUsersId(userId);

            List<CheepDTO> followingCheeps = new List<CheepDTO>();
            int followedUsersCheepsCount = 0;

            foreach(int id in FollowedUsers) {
                followingCheeps.AddRange(await _authorService.GetCheepsByAuthorId(id, offset, limit));
                followedUsersCheepsCount += await _authorService.GetCheepsCountsFromAuthorId(id);
            }

            if (userName == author) // logged-in user's page
            {
                (UserCheeps, int cheepsCount) = await _authorService.GetCheepsByAuthor(author, offset, limit);
                Cheeps.Clear();
                Cheeps.AddRange(UserCheeps);
                Cheeps.AddRange(followingCheeps);
                ViewData["CheepsCount"] = cheepsCount + followedUsersCheepsCount - excessiveCheepsCount;
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

    public string? GetYouTubeEmbed(string message, out string Message)
    {
        string pattern =  @"(.*?)(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/(watch\?v=)?([^?&\n]{11})(?:[^\n ]*)(.*)";
        Match match = Regex.Match(message, pattern, RegexOptions.Singleline);

        if (match.Success)
        {
            var videoId = match.Groups[6].Value.Substring(0, 11);
            Message = match.Groups[1].Value.Trim() + " " + match.Groups[7].Value.Trim();
            return $"https://www.youtube.com/embed/{videoId}";
        }
        else
        {
            Message = message;
            return null;
        }
    }
    //hashtags
    //inspired from hashtag code from worklizard.com
    public List<string>? GetHashTags(string message, out string Message)
    {
        var regex = new Regex(@"(?<=#)\w+"); 
        var matches = regex.Matches(message);
        var hashTags = new List<string>();

        foreach (Match match in matches)
        {
            var formattedHashtag = $"/hashtag/{match.Value}";
            hashTags.Add(formattedHashtag);
            message = message.Replace("#" + match.Value, "");
        }

        Message = message;
        return hashTags.Count > 0 ? hashTags : null;
    }
}