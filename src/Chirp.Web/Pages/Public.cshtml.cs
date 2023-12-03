using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using Chirp.Web.ViewComponents;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep {get; set;} = new();

    [BindProperty]
    public NewFollow NewFollow {get; set;} = new();


    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;
    //readonly IReactionRepository<Reaction, Cheep, User> _reactionService;


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
        var userName = User.Identity.Name;

        try
        {
        await padlock.Lock();
        var author = await _authorService.GetAuthorByName(userName);

        // Create new auther if does not exist in database ready
        if (author is null) 
        {
            var user = await _userService.GetUserByName(userName);
            if (user is null) {
                await _userService.CreateUser(userName);
                user = await _userService.GetUserByName(userName);
            }
            await _authorService.CreateAuthor(user);
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
        var LoggedInUserName = User.Identity.Name;
        //var LoggedInUserEmail =  Add user email here and insert into the create user func
        var FollowedUserName = NewFollow.Author;
        
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

    //unfollow form button
    public async Task<IActionResult> OnPostUnfollow()
    {
        // Convert the username to Id
        var followerId = await _userService.GetUserIDByName(User.Identity.Name);
        var followingId = await _userService.GetUserIDByName(NewFollow.Author);

        var unfollowDTO = new FollowDTO(followerId, followingId);
            
        await _userService.UnfollowUser(unfollowDTO);

        return Redirect("/" + User.Identity.Name);
    }

    public async Task<int> FindUpvoteCountByCheepID(int id)
    {
        return 0;//await _reactionService.GetCheepsUpvoteCountsFromCheepID(id);
    }

    public async Task<int> FindUpdownCountByCheepID(int id)
    {
        return 0;//await _reactionService.GetCheepsUpdownCountsFromCheepID(id);
    }

    public async Task<IActionResult> OnPostReaction()
    {
        // the id for the user who is reacting
        /*var userId = await _userService.GetUserIDByName(User.Identity.Name);
        var cheepId = NewCheep.id;

        var newreact = new ReactionDTO()
        {
            cheepId = cheepId,
            userId = userId, 
            reactionType = "PLACEHOLDER"
        };

        await _reactionService.ReactToCheep(newreact);*/

        return Redirect("/" + User.Identity.Name);
    }
}

public class NewFollow 
{
    [Display(Name = "author")]
    public string? Author {get; set;} = string.Empty;
}

public class newReaction
{
    [Display(Name = "reaction")]
    public string? Reaction {get; set;} = string.Empty;
}
