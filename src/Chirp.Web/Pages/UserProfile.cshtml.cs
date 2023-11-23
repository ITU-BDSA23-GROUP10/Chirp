using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.Pages;

public class UserProfileModel : PageModel
{
    readonly IUserRepository<User> _userService;

public UserProfileModel(IUserRepository<User> userService)
    {
        _userService = userService;
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


}