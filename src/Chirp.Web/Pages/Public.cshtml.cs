using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using Chirp.Web.ViewComponents;
using System.Text.RegularExpressions;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep { get; set; } = new NewCheep { Message = string.Empty };

    [BindProperty]
    public NewFollow NewFollow { get; set; } = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;

    public List<CheepDTO> DisplayedCheeps { get; set; } = new List<CheepDTO>();

    public PublicModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep, User> authorService, IUserRepository<User> userService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userService = userService;
    }

    public async Task<IActionResult> OnPost()
    {
        AsyncPadlock padlock = new();
        var userName = User?.Identity?.Name ?? "default"; //throw new InvalidOperationException("401 Unauthorized: User not logged in.");

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

    public async Task<bool> CheckIfFollowed(int userId, int authorId)
    {
        return await _userService.IsFollowing(userId, authorId);
    }

    public async Task<int> FindUserIDByName(string userName)
    {
        return await _userService.GetUserIDByName(userName);
    }

    /* get method with pagination*/
    public async Task<ActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Page"] = page;

        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();
         
        try
        {
            await padlock.Lock();

            (DisplayedCheeps, int cheepsCount) = await _cheepService.GetSome(offset, limit);
            
            ViewData["CheepsCount"] = cheepsCount;
        }
        finally
        {
            padlock.Dispose();
        }

        return Page();
    }

    //follow form button
    public async Task<IActionResult> OnPostFollow() 
    {
        var LoggedInUserName = User?.Identity?.Name ?? "default";
        //var LoggedInUserEmail =  Add user email here and insert into the create user func
        var FollowedUserName = NewFollow.Author;
        
        // Check if followedUserName is null
        if (FollowedUserName == null)
        {
            throw new ArgumentNullException("Followed user does not exist.");
        }
        
        //Check if the user that is logged in exists
        try
        {
            var loggedInUser = await _userService.GetUserByName(LoggedInUserName);
            if (loggedInUser is null) {
                throw new Exception("User does not exist");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await _userService.CreateUser(LoggedInUserName);
        }

        var followerId = await _userService.GetUserIDByName(LoggedInUserName);
        var followingId = await _userService.GetUserIDByName(FollowedUserName);

        var followDTO = new FollowDTO(followerId, followingId);
        
        await _userService.FollowUser(followDTO);

        return Redirect("/" + LoggedInUserName);
    }

    //unfollow form button
    public async Task<IActionResult> OnPostUnfollow()
    {
        var userName = User?.Identity?.Name ?? "default";
        // Convert the username to Id
        if (string.IsNullOrEmpty(NewFollow.Author))
        {
            throw new ArgumentException("NewFollow.Author cannot be null or empty");
        }
        else
        {
            var followerId = await _userService.GetUserIDByName(userName);
            var followingId = await _userService.GetUserIDByName(NewFollow.Author);

            var unfollowDTO = new FollowDTO(followerId, followingId);
                
            await _userService.UnfollowUser(unfollowDTO);

            return Redirect("/" + userName);
        }
    }
    public string? GetYouTubeEmbed(string message, out string Message)
    {
        string pattern = @"(.*?)(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/(watch\?v=)?([^?&\n]+)(?:[^\n ]*)(.*)";
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
}

public class NewFollow 
{
    [Display(Name = "author")]
    public string? Author {get; set;} = string.Empty;
}