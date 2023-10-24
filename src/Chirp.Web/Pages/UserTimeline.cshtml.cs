using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorRepository<Author, Cheep> _service;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public UserTimelineModel(IAuthorRepository<Author, Cheep> service)
    {
        _service = service;
    }

    //get method with pagination
    public ActionResult OnGet(string author, [FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Author"] = author;
        ViewData["Page"] = page;

        int limit = 32;
        int offset = (page - 1) * limit;

        (Cheeps, ViewData["CheepsCount"]) = _service.GetAuthorsCheeps(author, offset, limit);

        return Page();
    }
}