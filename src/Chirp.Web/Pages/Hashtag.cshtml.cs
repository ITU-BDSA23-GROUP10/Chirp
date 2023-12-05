using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.Pages;

public class HashtagModel : PageModel
{
    public string TagName { get; set; }
    readonly IUserRepository<User> _userService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly ICheepRepository<Cheep, Author> _cheepService;

    public List<CheepDTO> cheeps { get; set; } = new List<CheepDTO>();

    public HashtagModel(IUserRepository<User> userService, IAuthorRepository<Author, Cheep, User> authorService,ICheepRepository<Cheep, Author> cheepService)
    {
        _userService = userService;
        _authorService = authorService;
        _cheepService = cheepService;
    }    

    public async Task<ActionResult> OnGetAsync(string tagName)
    {
        try
        {
        TagName = tagName;
        var userName = User?.Identity?.Name!;
        
        if(await _userService.GetUserByName(userName) == null)
        {
            await _userService.CreateUser(userName);
        }

        var user = await _userService.GetUserByName(userName);

        if (user is null)
        {
            throw new InvalidOperationException("User could not be created.");
        }

        //cheeps = await _authorService.GetAllCheepsByAuthorName(user.Name);
        cheeps = await _cheepService.GetCheepsByHashtag(tagName);

        return Page();
        
        }
        catch (NullReferenceException)
        {
            return Redirect("/");
        }
    }
}