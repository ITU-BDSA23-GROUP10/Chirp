using Chirp.Core;
using Microsoft.AspNetCore.Mvc;

//this makes Chirp.Web/Pages/Components/CheepList/default.cshtml viewable
public class CheepListViewComponent : ViewComponent 
{
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps) 
    {
        return View(cheeps);
    }
}