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
    readonly IAuthorRepository<Author, Cheep, User> _authorService;

    public List<User> following { get; set; } = new List<User>();

    public List<CheepDTO> cheeps { get; set; } = new List<CheepDTO>();

public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService)
{
    _userService = userService;
    _authorService = authorService;
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

        ViewData["Author"] = user.Name;

        cheeps = await _authorService.GetAllCheepsByAuthorName(user.Name);

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
            following.Add(await _userService.GetUserById(id));
        }
    }

    //TODO optional fun task by Helge =))))) 
    public async Task<IActionResult> OnPostDownloadData() {

        var folder = Path.Combine("wwwroot", "UserData");
        string filePathName = Path.Combine(folder, User.Identity.Name + "_UserData.json");

        var userID = await _userService.GetUserIDByName(User.Identity.Name);
        var username = User.Identity.Name;
        var email = (await _userService.GetUserByName(username)).Email;

        var followersIDs = await _userService.GetFollowedUsersId(userID);
        var followers = new List<string>();
        foreach (var id in followersIDs) {
            followers.Add((await _userService.GetUserById(id)).Name);
        }
        
        string[] userData;
        
        userData = new string[] {
            "User ID: " + userID,
            "Username: " + username,
            "Email: " + email,
            "Followers: " + string.Join(", ", followers)
        };
        try
        {
            System.IO.File.WriteAllLines(filePathName, userData);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
        

        byte[] filebytes = System.IO.File.ReadAllBytes(filePathName);

        return File(filebytes, "application/json", User.Identity.Name + "_UserData.json");
    }
}