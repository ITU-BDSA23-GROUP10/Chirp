using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using Chirp.Web.BindableClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace Chirp.Web.Pages;

// This is the page model for the UserProfile page (the user profile page)
// This page has the cheeps from the user account that is logged in as well as a list of users that the user is following
// As well as the ability to delete cheeps and download user data
// The user can also update their email on this page
// As well as the ability to delete their account (GDPR compliant)
public class UserProfileModel : BasePageModel
{

    [BindProperty]
    public NewEmail NewEmail { get; set; } = new NewEmail { Email = string.Empty};

    [BindProperty]
    public DeleteCheep DeleteThisCheep { get; set; } = new();

    public List<User> following { get; set; } = new List<User>();

    public List<CheepDTO> cheeps { get; set; } = new List<CheepDTO>();

    public UserProfileModel(
        ICheepRepository<Cheep, Author> cheepService,
        IAuthorRepository<Author, Cheep, User> authorService,
        IUserRepository<User> userService,
        IReactionRepository<Reaction> reactionService,
        IFollowsRepository<Follows> followsService)
        : base(cheepService, authorService, userService, reactionService, followsService)
        {
        }   
    // This grabs the users data and displays it on the page
    public async Task<ActionResult> OnGetAsync()
    {
        try
        {
        var userName = User?.Identity?.Name!;
        
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
        catch (NullReferenceException)
        {
            return Redirect("/");
        }
    }
    // This method allows the user to delete their account (GDPR compliant)
    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        try
        {
        var userName = User?.Identity?.Name!;
        var user = await _userService.GetUserByName(userName);
        
        if (user is null)
        {
            throw new InvalidOperationException("User could not be created.");
        }
        
        await _followsService.DeleteAllFollowers(user.UserId);
        await _reactionService.deleteAllUserReactions(user.UserId);
        await _userService.DeleteUser(user);

        return await Logout();
        }
        catch (NullReferenceException)
        {
            return Redirect("/");
        }
    }
    // This method allows the user to logout
    // This was inspired by: https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-sign-in?tabs=aspnetcore
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
    // This method finds all the users that the user is following and displays them on the page
    public async Task findUserFollowingByUserID(int userId)
    {
        var followingIDs = await _followsService.GetFollowedUsersId(userId);
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
        var followersIDs = await _followsService.GetFollowedUsersId(userID);
        var followers = new List<string>();
        
        // This gets the followers names
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

        // This gets the cheeps and formats them for the JSON file
        foreach(var cheep in cheeps)
        {
            cheepFormated.Add("[" + cheep.Timestamp + "] - \"" + cheep.Message + "\"");
        }

        // This adds all the data to an array so that it can be written to the file
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
        // This downloads the file by using a memory stream which requires the data be converted to bytes 
        var bytes = Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(filePathName));
        MemoryStream ms = new MemoryStream(bytes);
        
        // This thread deletes the file after 10 seconds to ensure that the file is not left on the server
        Thread thread = new Thread(new ThreadStart(Worker));
        thread.Start();
        void Worker() {
            Thread.Sleep(10000);
            System.IO.File.Delete(filePathName);
        }
        
        // This returns the file to the user as a JSON file which automatically gets downloaded
        return File(fileStream: ms, "application/json", userName + "_UserData.json");
    }
    // This method allows the user to update their email
    public async Task<IActionResult> OnPostAddUpdateEmail()
    {
        // This checks if the user is authenticated which requires them to be logged in
        if(User?.Identity?.IsAuthenticated ?? false)
        {
            var userName = User?.Identity?.Name ?? "default";
            var user = await _userService.GetUserByName(userName);
            var email = user?.Email;
            
            // This checks if the user already has an email and if they do it adds it to a TempData variable 
            //(this is used to display the email on the page)
            if(email != null)
            {
                TempData["UserEmail"] = email;
            }
        }

        // This validates the email using the EmailValidator class (Fluent Validation)
        var validator = new EmailValidator();
        var result = validator.Validate(NewEmail);

        var duplicateEmail = await _userService.GetUserByEmail(NewEmail.Email);
        
        //TempData maintains the data when you move from one action to another action
        //usefull when you want to retain requests with http redirects

        // This checks if the email is valid and if it is not it adds an error message to TempData
        // Examples of invalid emails include bad formatting and duplicate emails 
        if(!result.IsValid)
        {
            TempData["EmailError"] = "Email formatting is incorrect";
            return Redirect("/Profile");
        }
        else if (duplicateEmail != null && duplicateEmail.Email == NewEmail.Email)
        {
            TempData["EmailError"] = "Duplicate email, that email already exists";
            return Redirect("/Profile");
        }

        // This updates the users email and adds a success message to TempData
        try
        {
            var userName = User?.Identity?.Name ?? "default";
            await _userService.UpdateUserEmail(userName, NewEmail.Email);
            TempData["UserEmail"] = NewEmail.Email;  // Update TempData with the new email
            TempData["EmailError"] = "Email successfully updated";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            TempData["EmailError"] = "Error updating email";
        }

        return Redirect("/Profile");
    }

    // This method allows the user to delete a cheep
    public async Task<IActionResult> OnPostDeleteCheep()
    {
        await _cheepService.Delete(DeleteThisCheep.CheepID);
        // Re-adds the cheeps to the author.Cheeps
        cheeps = await _authorService.GetAllCheepsByAuthorName(User?.Identity?.Name!);
        return Redirect("/Profile");
    }
}