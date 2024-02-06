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
            Koszyk koszyk = new Koszyk(_context, this.HttpContext); //dostaje baze danych i identygikator przeglqdarki
                                                                      //ten obiekt tworzony jest tylko po to, żeby w jednym obiekcie przekazać
                                                                      //dwa elementy: ElementyKoszyka, GetRazem
            DaneKoszyk daneDoKoszyka = new DaneKoszyk
            {
                ElementyKoszyka = koszyk.GetElementyKoszyka(),
                Razem = await koszyk.GetRazem(),
                IloscBiletow = await koszyk.GetIloscBiletow()
            };
            ViewBag.IloscBiletow = daneDoKoszyka.IloscBiletow;
            //przekazujemy dane do koszyka złozone z dwóch elementów do widoku
            return View(daneDoKoszyka);


        }
        //Funkcja kontrollera dodaje towar do koszyka
        //To jest akcja, która zostanie wywołana pod przyciskiem dodaj do koszyka
        public async Task<ActionResult> DodajDoKoszyka(int id)
        {
            // Sprawdź, czy bilet o podanym id istnieje
            var bilet = await _context.Bilety.FindAsync(id);
            if (bilet == null)
            {
                // Jeśli bilet nie istnieje, możesz obsłużyć to odpowiednim komunikatem lub przekierowaniem
                // W tym przypadku zakładam, że przekierowujesz z powrotem do widoku z błędem
                TempData["Error"] = "Bilet o podanym identyfikatorze nie został znaleziony.";
                return RedirectToAction("Index", "Home"); // Zmienić na odpowiednią akcję i kontroler
            }

            // Bilet został znaleziony, dodaj go do koszyka
            Koszyk koszyk = new Koszyk(_context, this.HttpContext);
            koszyk.DodajDoKoszyka(bilet);

            // Zmniejsz ilość dostępnych biletów o 1
            bilet.IloscBiletow--;

            // Zapisz zmiany w bazie danych
             _context.SaveChangesAsync();

            // Po dodaniu biletu do koszyka, przechodzę do widoku koszyka (Index)
            return RedirectToAction("Index", "Koszyk");
        }
        [HttpPost]
      
public ActionResult UsunZKoszyka(int id)
        {
            // Pobierz koszyk z sesji
            var koszyk = new Koszyk(_context, this.HttpContext);

            // Pobierz element koszyka z bazy danych
            var elementKoszykaWithBilet = _context.ElementKoszyka.Include(e => e.Bilety).FirstOrDefault(e => e.IdElementuKoszyka == id);

            if (elementKoszykaWithBilet != null)
            {
                // Pobierz bilet przypisany do elementu koszyka
                var bilet = elementKoszykaWithBilet.Bilety;
                

                if (bilet != null)
                {
                    // Zwiększ ilość dostępnych biletów o 1
                    bilet.IloscBiletow++;

                    // Usuń z koszyka
                    koszyk.UsunZKoszyka(id);

                    // Zapisz zmiany w bazie danych
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        //public PartialViewResult PodsumowanieKoszyka()
        //{
        //    var koszyk = new Koszyk(_context, this.HttpContext);
        //    int iloscBiletowWKoszyku = koszyk.GetIloscBiletow().Result;

        //    ViewData["iloscBiletowWKoszyku"] = iloscBiletowWKoszyku;
     
        //    return PartialView("_PodsumowanieKoszyka", iloscBiletowWKoszyku);
        //}
        public ActionResult PodsumowanieKoszyka()
        {
            var koszyk = new Koszyk(_context, this.HttpContext);
            int iloscBiletowWKoszyku = koszyk.GetIloscBiletow().Result;

            ViewData["iloscBiletowWKoszyku"] = iloscBiletowWKoszyku;
            @ViewBag.IloscBiletow = iloscBiletowWKoszyku;
            return View();
        }

    }
}

