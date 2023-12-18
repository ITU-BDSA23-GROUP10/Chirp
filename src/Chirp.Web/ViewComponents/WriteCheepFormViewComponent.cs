using Microsoft.AspNetCore.Mvc;
using Chirp.Web.BindableClasses;

namespace Chirp.Web.ViewComponents
{
    public class WriteCheepFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(NewCheep newCheep)
        {
            return View(newCheep);
        }
    }
}