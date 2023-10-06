using Microsoft.AspNetCore.Mvc;
using Chirp.Razor;
using System.Threading.Tasks;
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

        public async Task<IViewComponentResult> InvokeAsync(string? author = null)
        {
            int page = (int)ViewData["Page"];

            // from Mike Brind: https://www.mikesdotnetting.com/article/328/simple-paging-in-asp-net-core-razor-pages
            var count = await _service.GetCount(author);
            int _count = count > 0 ? count : 1;

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
