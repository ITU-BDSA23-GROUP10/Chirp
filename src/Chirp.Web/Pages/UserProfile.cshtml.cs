using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;



namespace Chirp.Web.Pages;

public class UserProfileModel : PageModel
{
    readonly IUserRepository<User> _userService;


public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService)
    {
        _userService = userService;
    }
    

    public async Task<ActionResult> OnGetAsync() {
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