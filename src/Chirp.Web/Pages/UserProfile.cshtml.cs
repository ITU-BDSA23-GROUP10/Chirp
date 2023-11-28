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

public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService)
{
    _userService = userService;
}    

    public async Task<ActionResult> OnGetAsync()
    {
        var userName = User?.Identity?.Name ?? "default";
        if(User?.Identity?.IsAuthenticated == false) {
            return Redirect("/");
        }
        
        if(await _userService.GetUserByName(userName) == null) {
            await _userService.CreateUser(userName);
        }

        var user = await _userService.GetUserByName(userName);
        
        ViewData["UserName"] = user.Name;
        ViewData["UserEmail"] = user.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        if(User?.Identity?.IsAuthenticated == false) {
            return Redirect("/");
        }

        var userName = User?.Identity?.Name ?? "default";
        var user = await _userService.GetUserByName(userName);
        await _userService.DeleteAllFollowers(user.UserId);
        await _userService.DeleteUser(user);

        return await Logout();
    }
    // This was inspired by: https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-sign-in?tabs=aspnetcore
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}