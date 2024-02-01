using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Platnosc(Zamowienie zamowienie)
        {
            try
            {
                var order = new Zamowienie
                {
                    Imie = zamowienie.Imie,
                    Nazwisko = zamowienie.Nazwisko,
                    Email = zamowienie.Email,
                    // ... inne pola zamówienia
                };

                if (string.Equals(zamowienie.Imie, kodPromocyjny, StringComparison.OrdinalIgnoreCase) == false)
                {
                    ViewBag.ErrorMessage = "Invalid promo code.";
                    return View(order);
                }
                else
                {
                    order.Imie = User.Identity.Name;
                    order.OrderDate = DateTime.Now;

                    // Zapisz zamówienie
                    _context.Zamowienie.Add(order);
                    _context.SaveChanges();

                    // Pobierz elementy koszyka dla zalogowanego użytkownika
                    var loggedInUserCartItems = _context.ElementKoszyka
                        .Where(c => c.IdSesjiKoszyka == User.Identity.Name)
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
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your order. Please try again.";
                return View();
            }
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
