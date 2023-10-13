using Microsoft.AspNetCore.Mvc;
using Chirp.Razor;
using System.Threading.Tasks;
using Chirp.Core;
namespace Chirp.Razor.ViewComponents
{
    // Overall ViewComponent code refactored from: https://www.learnrazorpages.com/razor-pages/view-components

    public class PaginationViewComponent : ViewComponent
    {
        private readonly ICheepService _service;
        public PaginationViewComponent(ICheepService cheepService)
        {
            _service = cheepService;
        }

        public IViewComponentResult Invoke(string? author = null)
        {
            int page = (int)ViewData["Page"];
            int _count = (int)ViewData["CheepsCount"];

            PagesData pagesData = new()
            {
                TotalPages = (int)Math.Ceiling((double)_count / _service.GetLimit()),
                CurrentPage = page,
            };

            if (author == null)
            {
                pagesData.HasAuthor = false;
            }

            pagesData.LastPage = (page == pagesData.TotalPages);

            return View(pagesData);
        }
    }
}