﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpRazor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel>? Cheeps { get; set; }

    /*public int CurrentPage { get; set; } = 1;
    public bool LastPage { get; set; } = false;
    public int TotalPages { get; set; } = 1;*/

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    /* get method with pagination*/
    public ActionResult OnGet([FromQuery(Name = "page")] int page = 1)
    {
        ViewData["Page"] = page;

        (Cheeps, ViewData["CheepsCount"]) = _service.GetCheeps(page);

        return Page();
    }
}