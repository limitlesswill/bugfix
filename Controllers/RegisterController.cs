using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebSite.Dashboard.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult register()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
