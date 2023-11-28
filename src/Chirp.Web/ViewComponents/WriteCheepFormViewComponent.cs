using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.ViewComponents;
public class WriteCheepFormViewComponent : ViewComponent
{
    private readonly IAuthorRepository<Author, Cheep, User> _authorService;
    private readonly ICheepRepository<Cheep, Author>  _cheepService;

    protected NewCheep NewestCheep {get; set;} = new();

    public WriteCheepFormViewComponent(IAuthorRepository<Author, Cheep, User> authorService, ICheepRepository<Cheep, Author> cheepService)
    {
        _authorService = authorService;
        _cheepService = cheepService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string userName, string newCheepMessage)
    {
        AsyncPadlock padlock = new();
        CheepCreateDTO cheep;
        Author author;

        try
        {
            await padlock.Lock();
            author = await _authorService.GetAuthorByName(userName);

            if (author is null) 
            {
                //await _authorService.CreateAuthor(userName);
                author = await _authorService.GetAuthorByName(userName);
            }

            cheep = new CheepCreateDTO(newCheepMessage, userName);
            
            await _cheepService.CreateCheep(cheep, author);
        }
        finally
        {
            NewestCheep.Message = newCheepMessage;
            padlock.Dispose();
        }

        return View(NewestCheep);
    }
}

public class NewCheep 
{
    //annotations https://www.bytehide.com/blog/data-annotations-in-csharp
    [MaxLength(160)]
    [Display(Name = "text")]
    [Required(ErrorMessage = "Cheep message cannot be empty.")]
    public string? Message {get; set;} = string.Empty;
}
