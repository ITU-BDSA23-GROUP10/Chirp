using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep {get; set;} = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep> _authorService;

    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public PublicModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep> authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
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
            await _authorService.CreateAuthor(userName);
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

    /* get method with pagination*/
    public async Task<ActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Page"] = page;

        int limit = PagesData.CheepsPerPage;
        int offset = (page - 1) * limit;

        AsyncPadlock padlock = new();
        try
        {
            await padlock.Lock();

            (Cheeps, int cheepsCount) = await _cheepService.GetSome(offset, limit);
            ViewData["CheepsCount"] = cheepsCount;
        }
        finally
        {
            padlock.Dispose();
        }

        return Page();
    }
}

public class NewCheep 
{
    //annotations https://www.bytehide.com/blog/data-annotations-in-csharp
    [MaxLength(160)]
    [Display(Name = "text")]
    public string? Message {get; set;} = string.Empty;
}