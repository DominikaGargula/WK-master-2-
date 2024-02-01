using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;

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
        [HttpPost]
        public IActionResult Platnosc(Zamowienie zamowienie, string kodPromocyjny)
        {
            
                int uzytkownikId = 0;
                var uzytkownikIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (uzytkownikIdClaim != null && int.TryParse(uzytkownikIdClaim, out var parsedId))
                {
                    uzytkownikId = parsedId;
                }

                var order = new Zamowienie
                {
                    Imie = zamowienie.Imie,
                    Nazwisko = zamowienie.Nazwisko,
                    Email = User.Identity.Name,
                    UzytkownikId = uzytkownikId, // Ustawienie UzytkownikId
                    OrderDate = DateTime.Now,
                    // ... inne pola zamówienia
                };

                if (string.Equals(kodPromocyjny, kodPromocyjny, StringComparison.OrdinalIgnoreCase) == false)
                {
                    ViewBag.ErrorMessage = "Invalid promo code.";
                    return View(order);
                }


                // Zapisz zamówienie
                _context.Zamowienie.Add(order);
                _context.SaveChanges();

                // Pobierz elementy koszyka dla zalogowanego użytkownika
                var loggedInUserCartItems = _context.ElementKoszyka
                    .Where(c => c.IdSesjiKoszyka == User.Identity.Name)
                    .Include(c => c.Bilety)
                    .Include(c=>c.Bilety.Wydarzenie)
                    .ToList();

                // Przypisz elementy koszyka do zamówienia
                foreach (var cartItem in loggedInUserCartItems)
                {
                    var szczegolZamowienia = new ZamowienieSzczegoly
                    {
                        IdZamowienie = order.IdZamowienie,
                        IdBilet = cartItem.IdBilet,
                        Ilosc = cartItem.Ilosc,
                        Cena = cartItem.Bilety.Wydarzenie.Cena // Uzupełnij odpowiednią właściwość zależnie od Twojego modelu
                                                               // ... inne pola szczegółów zamówienia
                    };

                    _context.ZamowienieSzczegoly.Add(szczegolZamowienia);
                }

                // Wyczyść koszyk użytkownika
                _context.ElementKoszyka.RemoveRange(loggedInUserCartItems);
                _context.SaveChanges();

                return RedirectToAction("Complete", new { id = order.IdZamowienie });
       
        }
        public IActionResult Zakoncz(int id)
        {
            // Pobierz zamówienie z identyfikatorem id z bazy danych
            var zamowienie = _context.Zamowienie
                .Include(z => z.ZamowienieSzczegolu)
                .FirstOrDefault(z => z.IdZamowienie == id);

            // Sprawdź, czy zamówienie istnieje i czy należy do aktualnie zalogowanego użytkownika
            bool isValid = zamowienie != null &&
                           zamowienie.Email == User.Identity.Name;

            if (isValid)
            {
                return View(zamowienie);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
