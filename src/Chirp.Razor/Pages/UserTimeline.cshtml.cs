using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public bool LastPage { get; set; } = false;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    //get method with pagination
    public async Task<ActionResult> OnGet(string author, [FromQuery(Name = "page")] int page = 1)
    {
        var count = await _service.GetCount();
        int _count = count > 0 ? count : 1;
        TotalPages = (int)Math.Ceiling((double)_count / _service.GetLimit());
        
        Cheeps = _service.GetCheepsFromAuthor(author, page);
        CurrentPage = page;

        if (CurrentPage == TotalPages)
        {
            LastPage = true;
        }

        return Page();
    }
}
