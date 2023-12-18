using Chirp.Core;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;
public class CheepListViewComponent : ViewComponent 
{
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps) 
    {
        return View(cheeps);
    }
}