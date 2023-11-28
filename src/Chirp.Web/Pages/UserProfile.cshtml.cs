using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Chirp.Web.Pages;

public class UserProfileModel : PageModel
{
    readonly IUserRepository<User> _userService;

    public List<User> following { get; set; } = new List<User>();

public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService)
{
    _userService = userService;
}    

    public async Task<ActionResult> OnGetAsync()
    {
        if(!User.Identity.IsAuthenticated) {
            return Redirect("/");
        }
        
        if(await _userService.GetUserByName(User.Identity.Name) == null) {
            await _userService.CreateUser(User.Identity.Name);
        }

        var loggedInUserId = await FindUserIDByName(User.Identity.Name);
        await findUserFollowingByUserID(loggedInUserId);

        var userName = User.Identity.Name;
        var user = await _userService.GetUserByName(userName);
        
        ViewData["UserName"] = user.Name;
        ViewData["UserEmail"] = user.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        if(!User.Identity.IsAuthenticated) {
            return Redirect("/");
        }

        var userName = User.Identity.Name;
        var user = await _userService.GetUserByName(userName);
        await _userService.DeleteAllFollowers(user.UserId);
        _userService.DeleteUser(user);

        return await Logout();
    }
    // This was inspired by: https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-sign-in?tabs=aspnetcore
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    public async Task<int> FindUserIDByName(string userName)
    {
        return await _userService.GetUserIDByName(userName);
    }

    public async Task findUserFollowingByUserID(int userId)
    {
        var followingIDs = await _userService.GetFollowedUsersId(userId);
        foreach (var id in followingIDs) {
            // check if user/id is in the list
            following.Add(_userService.GetUserById(id));
        }
    }
}