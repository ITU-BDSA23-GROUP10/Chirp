using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository<Cheep, Author> _service;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public PublicModel(ICheepRepository<Cheep, Author> service)
    {
        _service = service;
    }

    /* get method with pagination*/
    public ActionResult OnGet([FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Page"] = page;

        int limit = 32;
        int offset = (page - 1) * limit;

        (Cheeps, ViewData["CheepsCount"]) = _service.GetSome(offset, limit);

        return Page();
    }
}