using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebSite.Dashboard.Controllers
{
    public class NotFoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
