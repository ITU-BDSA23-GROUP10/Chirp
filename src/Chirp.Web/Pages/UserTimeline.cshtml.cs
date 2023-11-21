using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep {get; set;} = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep, User> _authorService;
    readonly IUserRepository<User> _userService;

    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public UserTimelineModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep, User> authorService, IUserRepository<User> userService)
    {
        _authorService = authorService;
        _cheepService = cheepService;
        _userService = userService;
    }

    public async Task<IActionResult> OnPost()
    {

        AsyncPadlock padlock = new();
        var userName = User.Identity.Name;

        try
        {
        await padlock.Lock();
        var author = await _authorService.GetAuthorByName(userName);

        // Create new auther if does not exist in database ready
        if (author is null) 
        {
            await _authorService.CreateAuthor(await _userService.GetUserByName(userName));
            author = await _authorService.GetAuthorByName(userName);
        }

        var cheep = new CheepCreateDTO(NewCheep.Message, userName);
        
        await _cheepService.CreateCheep(cheep, author);

        }
        finally
        {
            padlock.Dispose();
        }

        return Redirect("/" + userName);
    }

    //get method with pagination
    public async Task<ActionResult> OnGetAsync(string author, [FromQuery(Name = "page")] int page = 1)
    {

        ViewData["Author"] = author;
        ViewData["Page"] = page;

        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();

        try
        {
            await padlock.Lock();
            (Cheeps, int cheepsCount) = await _authorService.GetCheepsByAuthor(author, offset, limit);
            ViewData["CheepsCount"] = cheepsCount;
        }
        finally
        {
            padlock.Dispose();
        }

        return Page();
    }
}