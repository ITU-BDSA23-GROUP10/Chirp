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
    readonly IReactionRepository<Reaction> _reactionService;

    public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService, IReactionRepository<Reaction> reactionService)
    {
        _userService = userService;
        _reactionService = reactionService;
    }    

    public async Task<ActionResult> OnGetAsync()
    {
        if(!User.Identity.IsAuthenticated) {
            return Redirect("/");
        }
        
        if(await _userService.GetUserByName(User.Identity.Name) == null) {
            await _userService.CreateUser(User.Identity.Name);
        }

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
        await _reactionService.deleteAllUserReactions(user.UserId);
        _userService.DeleteUser(user);

        return await Logout();
    }

    // This was inspired by: https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-sign-in?tabs=aspnetcore
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}