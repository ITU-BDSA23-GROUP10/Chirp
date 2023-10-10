using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Razor;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel>? Cheeps { get; set; }

    /*public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public bool LastPage { get; set; } = false;
    public bool CurrentAuthor { get; set; }*/

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    //get method with pagination
    public ActionResult OnGet(string author, [FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Author"] = author;
        ViewData["Page"] = page;

        (Cheeps, ViewData["CheepsCount"]) = _service.GetCheepsFromAuthor(author, page);

        return Page();
    }
}