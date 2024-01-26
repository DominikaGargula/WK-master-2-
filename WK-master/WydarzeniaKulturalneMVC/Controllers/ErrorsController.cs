using Microsoft.AspNetCore.Mvc;

namespace Przychodnia.App.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error401()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
    }
}
