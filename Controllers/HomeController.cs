using Microsoft.AspNetCore.Mvc;

namespace MSU_BARODA.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
