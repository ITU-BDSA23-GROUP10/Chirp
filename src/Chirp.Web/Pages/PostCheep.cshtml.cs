using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.Pages1;

public class PostCheepModel : PageModel
{
    [BindProperty]
    public NewCheep NewCheep {get; set;} = new();

    readonly ICheepRepository<Cheep, Author> _cheepService;
    readonly IAuthorRepository<Author, Cheep> _authorService;
    
    public PostCheepModel(ICheepRepository<Cheep, Author> cheepService, IAuthorRepository<Author, Cheep> authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    public ActionResult OnGet()
    {
        
        if (User.Identity is null || User.Identity.Name is null)
        {
            // No login? Go back public page.
            // Should be sign-up or register page later.
            return Redirect("https://localhost:5273");
        }

        return Page();

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

        return Redirect("https://localhost:5273/" + userName);
    }
}


public class NewCheep 
{
    //annotations https://www.bytehide.com/blog/data-annotations-in-csharp
    [MaxLength(160)]
    [Display(Name = "text")]
    public string? Message {get; set;} = string.Empty;
}
