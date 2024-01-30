using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WydarzeniaKulturalne.Data;

namespace WydarzeniaKulturalneMVC.Controllers
{
    [Authorize]
    public class FinalizacjaKoszykaController : Controller
    {

        private readonly WydarzeniaKulturalneContext _context;
        string kodPromocyjny = "WSB";
        public FinalizacjaKoszykaController(WydarzeniaKulturalneContext context)
        {

            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Platnosc()
        {
            return View();
        }
    }
}
