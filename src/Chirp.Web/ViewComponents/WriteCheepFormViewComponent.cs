using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Web.ViewComponents;
public class WriteCheepFormViewComponent : ViewComponent
{
    private readonly IAuthorRepository<Author, Cheep, User> _authorService;
    private readonly ICheepRepository<Cheep, Author>  _cheepService;

    protected NewCheep NewestCheep { get; set; } = new NewCheep { Message = string.Empty };


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

            if(newCheepMessage is null || newCheepMessage.Length < 1)
            {
                ViewData["CheepTooShort"] = "true";
                padlock.Dispose();
                return View(NewestCheep);
            } else 
            {
                ViewData["CheepTooShort"] = "false";
                cheep = new CheepCreateDTO(newCheepMessage, userName);
                await _cheepService.CreateCheep(cheep, author);
            }

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
    [Required]
    [MaxLength(160)]
    [Display(Name = "text")]
    public required string Message {get; set;} = string.Empty;
}
