using Microsoft.AspNetCore.Mvc;

namespace ThalysonProjetoWEB.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
