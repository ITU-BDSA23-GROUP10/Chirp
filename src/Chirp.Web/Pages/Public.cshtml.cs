using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using Chirp.Web.ViewComponents;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep { get; set; } = new NewCheep { Message = string.Empty };

    [BindProperty]
    public NewFollow NewFollow { get; set; } = new();

    [BindProperty]
    public NewcheepId NewcheepId {get; set;} = new();
        
    [BindProperty]
    public NewReaction NewReaction {get; set;} = new();


    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;
    readonly IReactionRepository<Reaction> _reactionService;


    public List<CheepDTO> DisplayedCheeps { get; set; } = new List<CheepDTO>();

    public PublicModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep, User> authorService, IUserRepository<User> userService, IReactionRepository<Reaction> reactionService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _userService = userService;
        _reactionService = reactionService;
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

    public async Task<int> FindUpvoteCountByCheepID(int id)
    {
        return await _reactionService.GetCheepsUpvoteCountsFromCheepID(id);
    }

    public async Task<int> FindDownvoteCountByCheepID(int id)
    {
        return await _reactionService.GetCheepsDownvoteCountsFromCheepID(id);
    }

    public async Task<IActionResult> OnPostReaction()
    {
        // the id for the user who is reacting
        var userId = await _userService.GetUserIDByName(User.Identity.Name);
        int cheepId = NewcheepId.id  ?? default(int);
        string react = NewReaction.Reaction;

        // Checks if the user exists
        try
        {
            if(userId == -1 && User.Identity.Name != null) {
                await _userService.CreateUser(User.Identity.Name);
                userId = await _userService.GetUserIDByName(User.Identity.Name); 
            }
        } catch (Exception e) 
        {
            Console.WriteLine(e.Message);
            throw new Exception("There was a problem whilst creating the user");
        }
        //throw new Exception("UserID:  " + userId + "  ||  cheepID: " + cheepId + "   ||   Reaction: " + NewReaction.Reaction);

        var newreact = new ReactionDTO
        (
            cheepId,
            userId, 
            react
        );

        //throw new Exception("UserID:  " + newreact.userId + "  ||  cheepID: " + newreact.cheepId + "   ||   Reaction: " + newreact.reactionType);

        await _reactionService.ReactToCheep(newreact);

        return Redirect("/" + User.Identity.Name);
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

public class NewFollow 
{
    [Display(Name = "author")]
    public string? Author {get; set;} = string.Empty;
}

