using Microsoft.AspNetCore.Mvc;
using WydarzeniaKulturalneMVC.ViewModel;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalneMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WydarzeniaKulturalneMVC.Controllers
{

    public class KoszykController : Controller
    {
        private readonly WydarzeniaKulturalneContext _context;
        public KoszykController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            Koszyk koszyk = new Koszyk(_context, this.HttpContext); 

            DaneKoszyk daneDoKoszyka = new DaneKoszyk
            {
                ElementyKoszyka = koszyk.GetElementyKoszyka(),
                Razem = await koszyk.GetRazem(),
                IloscBiletow = await koszyk.GetIloscBiletow()
            };
            ViewBag.IloscBiletow = daneDoKoszyka.IloscBiletow;

            return View(daneDoKoszyka);

        }

        public async Task<ActionResult> DodajDoKoszyka(int id)
        {
            var bilet = await _context.Bilety.FindAsync(id);
            if (bilet == null)
            {
                TempData["Error"] = "Bilet o podanym identyfikatorze nie został znaleziony.";
                return RedirectToAction("Index", "Home");
            }
            Koszyk koszyk = new Koszyk(_context, this.HttpContext);
            koszyk.DodajDoKoszyka(bilet);
            bilet.IloscBiletow--;
            _context.SaveChangesAsync();

            return RedirectToAction("Index", "Koszyk");
        }

        [HttpPost]

        public ActionResult UsunZKoszyka(int id)
        {
            var koszyk = new Koszyk(_context, this.HttpContext);

            var elementKoszykaWithBilet = _context.ElementKoszyka.Include(e => e.Bilety).FirstOrDefault(e => e.IdElementuKoszyka == id);

            if (elementKoszykaWithBilet != null)
            {
                var bilet = elementKoszykaWithBilet.Bilety;

                if (bilet != null)
                {
                    bilet.IloscBiletow++;

                    koszyk.UsunZKoszyka(id);

                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult PodsumowanieKoszyka()
        {
            var koszyk = new Koszyk(_context, this.HttpContext);
            int iloscBiletowWKoszyku = koszyk.GetIloscBiletow().Result;

            ViewData["iloscBiletowWKoszyku"] = iloscBiletowWKoszyku;
            ViewBag.IloscBiletow = iloscBiletowWKoszyku;
            return View();
        }

    }
}

