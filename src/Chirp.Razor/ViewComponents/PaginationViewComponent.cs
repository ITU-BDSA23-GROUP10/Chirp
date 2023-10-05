using Microsoft.AspNetCore.Mvc;
using Chirp.Razor;
using System.Threading.Tasks;
namespace Chirp.Razor.ViewComponents
{
    // Overall ViewComponent code refactored from: https://www.learnrazorpages.com/razor-pages/view-components

    public class PagesData {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages  { get; set; } = 1;
        public bool LastPage   { get; set; } = false;
        public bool HasAuthor { get; set; } = false;
    }

    public class PaginationViewComponent : ViewComponent
    {
        private readonly ICheepService _service;
        public PaginationViewComponent(ICheepService cheepService)
        {
            _service = cheepService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool hasAuthor = false)
        {
            string? author = ViewData["Author"] as string;
            int page = (int)ViewData["Page"];

            // from Mike Brind: https://www.mikesdotnetting.com/article/328/simple-paging-in-asp-net-core-razor-pages
            var count = !hasAuthor ? await _service.GetCount() : await _service.GetCount(author);
            int _count = count > 0 ? count : 1;

            PagesData pagesData = new()
            {
                TotalPages = (int)Math.Ceiling((double)_count / _service.GetLimit()),
                CurrentPage = page,
                HasAuthor = hasAuthor
            };

            pagesData.LastPage = (page == pagesData.TotalPages);

            return View(pagesData);
        }
    }
}
