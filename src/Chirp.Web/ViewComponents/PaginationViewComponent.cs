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
            int page = -1;
            int _count = -1;
            
            try
            {
                page = (int)(ViewData["Page"] ?? -1);
                _count = (int)(ViewData["CheepsCount"] ?? -1);
            }
            catch(Exception e)
            {
                throw new Exception("int conversion went wrong in the pagination: " + e.ToString());
            }
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