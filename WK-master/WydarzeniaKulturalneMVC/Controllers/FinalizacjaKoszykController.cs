using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalneMVC.Models;

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

            var koszykElementy = _context.ElementKoszyka
                                 .Include(e => e.Bilety)
                                 .Include(e=> e.Bilety.Wydarzenie)
                                 .Where(e => e.IdSesjiKoszyka == User.Identity.Name).ToList();


            decimal sumaZamowienia = _context.ElementKoszyka
                                 .Where(e => e.IdSesjiKoszyka == User.Identity.Name)
                                 .Sum(item => item.Bilety.Wydarzenie.Cena * item.Ilosc);

            var order = new Zamowienie
            {

                Imie = zamowienie.Imie,
                Nazwisko = zamowienie.Nazwisko,
                Email = User.Identity.Name,
                UzytkownikId = uzytkownikId, // Ustawienie UzytkownikId
                OrderDate = DateTime.Now,
                Suma = sumaZamowienia
                // ... inne pola zamówienia
            };

            if (string.Equals(kodPromocyjny, kodPromocyjny, StringComparison.OrdinalIgnoreCase) == false)
            {
                ViewBag.ErrorMessage = "Invalid promo code.";
                return View(order);
            }

            try
            {
                // Zapisz zamówienie
                _context.Zamowienie.Add(order);
                _context.SaveChanges();

                // Pobierz elementy koszyka dla zalogowanego użytkownika
                var loggedInUserCartItems = _context.ElementKoszyka
                        .Where(c => c.IdSesjiKoszyka == User.Identity.Name)
                     
                        .Include(c => c.Bilety)
                        .Include(c => c.Bilety.Wydarzenie)
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
            }
            catch (Exception ex)
            {
                // Logowanie błędu - zależnie od systemu logowania
                // _logger.LogError("Błąd przy zapisie zamówienia: ", ex);

                ViewBag.ErrorMessage = "Wystąpił problem podczas przetwarzania zamówienia. Spróbuj ponownie.";
                return View(zamowienie);
            }

            //return RedirectToAction("Complete", new { id = order.IdZamowienie });
            return View("Zakoncz");

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
        //[HttpPost]
        //public IActionResult Platnosc(Zamowienie zamowienie, string kodPromocyjny)
        //{
        //    int uzytkownikId = PobierzIdUzytkownika();
        //    if (uzytkownikId == 0)
        //    {
        //        // Możesz dodać obsługę błędu, np. przekierowanie na stronę logowania
        //        return View("Error", new ErrorViewModel { RequestId = "UserIDNotFound" });
        //    }

        //    Zamowienie order = StworzZamowienie(zamowienie, uzytkownikId);

        //    if (!SprawdzKodPromocyjny(kodPromocyjny))
        //    {
        //        ViewBag.ErrorMessage = "Nieprawidłowy kod promocyjny.";
        //        return View(zamowienie);
        //    }

        //    try
        //    {
        //        _context.Zamowienie.Add(order);
        //        _context.SaveChanges();

        //        var loggedInUserCartItems = PobierzElementyKoszyka();
        //        PrzypiszElementyKoszykaDoZamowienia(loggedInUserCartItems, order);

        //        WyczyscKoszyk(loggedInUserCartItems);

        //        // Tutaj możesz dodać logikę przekierowania po pomyślnej płatności
        //        return RedirectToAction("Zakoncz", new { id = order.IdZamowienie });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Logowanie błędu i obsługa
        //        ViewBag.ErrorMessage = "Wystąpił błąd podczas przetwarzania zamówienia. Spróbuj ponownie.";
        //        return View("Error", new ErrorViewModel { RequestId = ex.Message });
        //    }
        //}
        //private int PobierzIdUzytkownika()
        //{
        //    var uzytkownikIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //    if (uzytkownikIdClaim != null && int.TryParse(uzytkownikIdClaim, out var parsedId))
        //    {
        //        return parsedId;
        //    }

        //    return 0; // lub inna wartość, wskazująca na błąd
        //}
        //private Zamowienie StworzZamowienie(Zamowienie zamowienie, int uzytkownikId)
        //{
        //    return new Zamowienie
        //    {
        //        Imie = zamowienie.Imie,
        //        Nazwisko = zamowienie.Nazwisko,
        //        Email = User.Identity.Name,
        //        UzytkownikId = uzytkownikId,
        //        OrderDate = DateTime.Now,
        //        // ... inne pola zamówienia
        //    };
        //}
        //private bool SprawdzKodPromocyjny(string kodPromocyjny)
        //{
        //    string prawidlowyKodPromocyjny = "WSB"; // Zastąp właściwym kodem
        //    return string.Equals(kodPromocyjny, prawidlowyKodPromocyjny, StringComparison.OrdinalIgnoreCase);
        //}
        //private List<ElementKoszyka> PobierzElementyKoszyka()
        //{
        //    return _context.ElementKoszyka
        //        .Where(c => c.IdSesjiKoszyka == User.Identity.Name)
        //        .Include(c => c.Bilety)
        //        .Include(c => c.Bilety.Wydarzenie)
        //        .ToList();
        //}
        //private void PrzypiszElementyKoszykaDoZamowienia(List<ElementKoszyka> koszyk, Zamowienie zamowienie)
        //{
        //    foreach (var item in koszyk)
        //    {
        //        var szczegolyZamowienia = new ZamowienieSzczegoly
        //        {
        //            IdZamowienie = zamowienie.IdZamowienie,
        //            IdBilet = item.IdBilet,
        //            Ilosc = item.Ilosc,
        //            Cena = item.Bilety.Wydarzenie.Cena
        //            // ... inne pola szczegółów zamówienia
        //        };
        //        _context.ZamowienieSzczegoly.Add(szczegolyZamowienia);
        //    }
        //}

        //private void WyczyscKoszyk(List<ElementKoszyka> koszyk)
        //{
        //    _context.ElementKoszyka.RemoveRange(koszyk);
        //    _context.SaveChanges();
        //}

    }
}
