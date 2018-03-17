using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DemoUser.Controllers
{
    public class HomeController: Controller
    {
        // GET
        public IActionResult Index()
        {
            return View(new Dictionary<string, object>
                {["Placeholder"] = "Placeholder" });
        }
    }
}