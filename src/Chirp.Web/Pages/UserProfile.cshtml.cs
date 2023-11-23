using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.Pages;

public class UserProfileModel : PageModel
{
    readonly IUserRepository<User> _userService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly ICheepRepository<Cheep, Author> _cheepService;


public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService)
    {
        _userService = userService;
        _authorService = authorService;
        _cheepService = cheepService;
    }
    

    public async Task<ActionResult> OnGetAsync() {
        if(!User.Identity.IsAuthenticated) {
            return Redirect("/");
        }
        var userName = User.Identity.Name;
        var user = await _userService.GetUserByName(userName);
        ViewData["UserName"] = user.Name;
        ViewData["UserEmail"] = user.Email;
        return Page();
    }

    public async Task<IActionResult> OnPostForgetMeAsync() {
        if(!User.Identity.IsAuthenticated) {
            return Redirect("/");
        }
        var userName = User.Identity.Name;
        var user = await _userService.GetUserByName(userName);
        await _userService.DeleteAllFollowers(user.UserId);
        _userService.DeleteUser(user);
        return Redirect("/");
    }

}