using Microsoft.AspNetCore.Mvc;
namespace Chirp.Web.ViewComponents
{
    public class FollowNotificationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}