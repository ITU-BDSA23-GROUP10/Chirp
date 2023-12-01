using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FluentValidation;


namespace Chirp.Web.Pages;

public class UserProfileModel : PageModel
{
    
    readonly IUserRepository<User> _userService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    [BindProperty]
    public NewEmail NewEmail { get; set; } = new NewEmail { Email = string.Empty};

    public List<User> following { get; set; } = new List<User>();

    public List<CheepDTO> cheeps { get; set; } = new List<CheepDTO>();

    public UserProfileModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService)
    {
        _userService = userService;
        _authorService = authorService;
    }    

    public async Task<ActionResult> OnGetAsync()
    {
        var userName = User?.Identity?.Name ?? "default";
        
        if(User?.Identity?.IsAuthenticated == false)
        {
            return Redirect("/");
        }
        
        if(await _userService.GetUserByName(userName) == null)
        {
            await _userService.CreateUser(userName);
        }

        var loggedInUserId = await FindUserIDByName(userName);
        await findUserFollowingByUserID(loggedInUserId);

        var user = await _userService.GetUserByName(userName);

        if (user is null)
        {
            throw new InvalidOperationException("User could not be created.");
        }
        
        ViewData["UserName"] = user.Name;
        ViewData["UserEmail"] = user.Email;

        ViewData["Author"] = user.Name;

        cheeps = await _authorService.GetAllCheepsByAuthorName(user.Name);

        return Page();
    }

    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        if(User?.Identity?.IsAuthenticated == false)
        {
            return Redirect("/");
        }

        var userName = User?.Identity?.Name ?? "default";
        var user = await _userService.GetUserByName(userName);
        
        if (user is null)
        {
            throw new InvalidOperationException("User could not be created.");
        }
        
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

    public async Task<int> FindUserIDByName(string userName)
    {
        return await _userService.GetUserIDByName(userName);
    }

    public async Task findUserFollowingByUserID(int userId)
    {
        var followingIDs = await _userService.GetFollowedUsersId(userId);
        foreach (var id in followingIDs)
        {
            var fetchedUser = await _userService.GetUserById(id);
            if(fetchedUser != null)
            {
                // check if user/id is in the list
                following.Add(fetchedUser);
            }
        }
    }

    // This downloads the user data as a JSON file
    public async Task<IActionResult> OnPostDownloadData()
    {
        var userName = User?.Identity?.Name ?? "default";

        // This is the filepath where the file will be saved
        var folder = Path.Combine("userDataFolder");
        Directory.CreateDirectory(folder);

        string filePathName = Path.Combine(folder, userName + "_UserData.json");

        // This is the data that will be saved
        var userID = await _userService.GetUserIDByName(userName);
        var user = await _userService.GetUserByName(userName);
        var email = user?.Email;
        var followersIDs = await _userService.GetFollowedUsersId(userID);
        var followers = new List<string>();
        
        foreach (var id in followersIDs)
        {
            var fetchedUser = await _userService.GetUserById(id);
            
            if(fetchedUser != null)
            {
                followers.Add(fetchedUser.Name);
            }
        }

        var cheeps = await _authorService.GetAllCheepsByAuthorName(userName);
        var cheepFormated = new List<string>();

        foreach(var cheep in cheeps)
        {
            cheepFormated.Add("[" + cheep.Timestamp + "] - \"" + cheep.Message + "\"");
        }

        // This saves the data to the file
        string[] userData;
        
        userData = new string[]
        {
            "User ID: " + userID,
            "Username: " + userName,
            "Email: " + email,
            "Followers: " + string.Join(", ", followers),
            "Cheeps: " + string.Join(", ", cheepFormated)
        };

        // This writes the data to the file
        try
        {
            System.IO.File.WriteAllLines(filePathName, userData);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }

        // This was taken from: https://stackoverflow.com/questions/72433767/return-json-file-from-api-endpoint
        // This downloads the file
        var bytes = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(filePathName));
        MemoryStream ms = new MemoryStream(bytes);
        
        Thread thread = new Thread(new ThreadStart(Worker));
        thread.Start();
        void Worker() {
            Thread.Sleep(10000);
            System.IO.File.Delete(filePathName);
        }
        
        return File(fileStream: ms, "application/json", userName + "_UserData.json");
    }

    public async Task<IActionResult> OnPostAddUpdateEmail()
    {
        if(User?.Identity?.IsAuthenticated ?? false)
        {
            var userName = User?.Identity?.Name ?? "default";
            var user = await _userService.GetUserByName(userName);
            var email = user?.Email;

            if(email != null)
            {
                ViewData["UserEmail"] = email;
            }
        }

        var validator = new EmailValidator();
        var result = validator.Validate(NewEmail);

        var duplicateEmail = await _userService.GetUserByEmail(NewEmail.Email);
        
        if(!result.IsValid)
        {
            ViewData["EmailError"] = "formatting";
            return Page();
        }
        else if (duplicateEmail != null && duplicateEmail.Email == NewEmail.Email)
        {
            ViewData["EmailError"] = "duplicate";
            return Page();
        }

        try
        {
            var userName = User?.Identity?.Name ?? "default";
            await _userService.UpdateUserEmail(userName, NewEmail.Email);
            ViewData["EmailError"] = "success";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            ViewData["EmailError"] = "error";
        }

        return Page();
    }
}
public class NewEmail 
{
    //annotations https://www.bytehide.com/blog/data-annotations-in-csharp
    [Required]
    [Display(Name = "email")]
    public required string Email {get; set;}
}

public class EmailValidator : AbstractValidator<NewEmail>
{
    public EmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}