using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalneMVC.Models;
using WydarzeniaKulturalneMVC.ViewModel;
namespace WydarzeniaKulturalneMVC.Controllers
{
    //[Authorize]
    public class FinalizacjaKoszykaController : Controller
    {

        private readonly WydarzeniaKulturalneContext _context;
        string kodPromocyjny;
        private Koszyk _koszyk;

        public FinalizacjaKoszykaController(WydarzeniaKulturalneContext context, IHttpContextAccessor httpContextAccessor)
        {
            _koszyk = new Koszyk(context, httpContextAccessor.HttpContext);
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var grupowaneSzczegoly = await _context.ZamowienieSzczegoly
            //.Include(sz => sz.Zamowienie) // Opcjonalne: Dołączenie encji Zamowienie, jeśli chcesz używać danych zamówienia
            //.Include(sz => sz.Bilet) // Opcjonalne: Dołączenie encji Bilet, jeśli chcesz używać danych biletu
            //.GroupBy(sz => sz.IdZamowienie)
            //.Select(grupa => new
            //{
            //    IdZamowienie = grupa.Key,
            //    Szczegoly = grupa.ToList()
            //})
            //.ToListAsync();

            var zamowienieSzczegolyList = _context.ZamowienieSzczegoly
             .Join(_context.Zamowienie,
                 szczegol => szczegol.IdZamowienie,
                 zamowienie => zamowienie.IdZamowienie,
                 (szczegol, zamowienie) => new { Szczegol = szczegol, Zamowienie = zamowienie })
             .Join(_context.Bilety,
                 szczegolZamowienie => szczegolZamowienie.Szczegol.IdBilet,
                 bilet => bilet.Id,
                 (szczegolZamowienie, bilet) => new { SzczegolZamowienie = szczegolZamowienie, Bilet = bilet })
             .OrderByDescending(x => x.SzczegolZamowienie.Szczegol.IdZamowienie)
             .Select(x => new
             {
                 IdZamowienieSzczegoly = x.SzczegolZamowienie.Szczegol.IdZamowienieSzczegoly,
                 IdZamowienie = x.SzczegolZamowienie.Szczegol.IdZamowienie,
                 IdBilet = x.SzczegolZamowienie.Szczegol.IdBilet,
                 Ilosc = x.SzczegolZamowienie.Szczegol.Ilosc,
                 Cena = x.SzczegolZamowienie.Szczegol.Cena,
                 NazwaBiletu = x.Bilet.Wydarzenie.Nazwa, // Zakładam, że właściwość Nazwa jest dostępna bezpośrednio w Bilety
                 DataWydarzenia = x.Bilet.DataWydarzenia,
                 ZdjecieUrl = x.Bilet.Wydarzenie.ZdjecieUrl,
                 Miejscowosc = x.Bilet.Lokalizacja.Miejscowosc,
                 NazwaLokalizacji = x.Bilet.Lokalizacja.NazwaMiejsca// Dodane założenie, że DataWydarzenia jest dostępna w Bilety
             })
             .ToList();

            ViewBag.SprzedaneBilety = zamowienieSzczegolyList;



            return View(zamowienieSzczegolyList);

        }
        public IActionResult Platnosc()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Platnosc(Zamowienie zamowienie, string kodPromocyjnyWpis)
        {
            var idSesjiKoszyka = _koszyk.IdSesjiKoszyka;
            int uzytkownikId = 0;
            var uzytkownikIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (uzytkownikIdClaim != null && int.TryParse(uzytkownikIdClaim, out var parsedId))
            {
                uzytkownikId = parsedId;
            }

            var koszykElementy = _context.ElementKoszyka
                                 .Include(e => e.Bilety)
                                 .Include(e => e.Bilety.Wydarzenie)
                                 .Where(e => e.IdSesjiKoszyka == idSesjiKoszyka).ToList();


            decimal sumaZamowienia = _context.ElementKoszyka
                                 .Where(e => e.IdSesjiKoszyka == idSesjiKoszyka)
                                 .Sum(item => item.Bilety.Wydarzenie.Cena * item.Ilosc);


            var order = new Zamowienie
            {

                Imie = zamowienie.Imie,
                Nazwisko = zamowienie.Nazwisko,
                Email = User.Identity.Name,
                UzytkownikId = uzytkownikId, // Ustawienie UzytkownikId
                OrderDate = DateTime.Now,
                Suma = sumaZamowienia

            };

            kodPromocyjny = "WSB";

            if (string.Equals(kodPromocyjny, kodPromocyjnyWpis, StringComparison.OrdinalIgnoreCase) == false)
            {
                TempData["ErrorMessagePlatnosc"] = "Błędny kod promocji.";
                return View(order);
            }

            try
            {
                // Zapisz zamówienie
                _context.Zamowienie.Add(order);
                _context.SaveChanges();

                // Pobierz elementy koszyka dla zalogowanego użytkownika
                var loggedInUserCartItems = _context.ElementKoszyka
                        .Where(c => c.IdSesjiKoszyka == idSesjiKoszyka)

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
                        Cena = cartItem.Bilety.Wydarzenie.Cena
                    };
                    _context.ZamowienieSzczegoly.Add(szczegolZamowienia);

                }

                // Wyczyść koszyk użytkownika


                _context.ElementKoszyka.RemoveRange(loggedInUserCartItems);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "Wystąpił problem podczas przetwarzania zamówienia. Spróbuj ponownie.";
                return View(zamowienie);
            }

            return RedirectToAction("Zakoncz", new { id = order.IdZamowienie });

        }

        public IActionResult Zakoncz(int id)
        {
            return View(id);

        }
        public IActionResult ListaSzczegolowZamowienPoEmail()
        {
            var userEmail = User.Identity.Name; // Pobranie e-maila zalogowanego użytkownika

            if (string.IsNullOrEmpty(userEmail))
            {
                // Użytkownik niezalogowany lub brak e-maila
                // Można zwrócić błąd lub pustą listę w zależności od wymagań aplikacji
                return View(new List<ZamowienieSzczegoly>());
            }

            var zamowienieSzczegolyList = _context.ZamowienieSzczegoly
             .Join(_context.Zamowienie,
                 szczegol => szczegol.IdZamowienie,
                 zamowienie => zamowienie.IdZamowienie,
                 (szczegol, zamowienie) => new { Szczegol = szczegol, Zamowienie = zamowienie })
             .Join(_context.Bilety,
                 szczegolZamowienie => szczegolZamowienie.Szczegol.IdBilet,
                 bilet => bilet.Id,
                 (szczegolZamowienie, bilet) => new { SzczegolZamowienie = szczegolZamowienie, Bilet = bilet })
             .Where(x => x.SzczegolZamowienie.Zamowienie.Email == userEmail)
             .OrderByDescending(x => x.SzczegolZamowienie.Szczegol.IdZamowienie)
             .Select(x => new
             {
                 IdZamowienieSzczegoly = x.SzczegolZamowienie.Szczegol.IdZamowienieSzczegoly,
                 IdZamowienie = x.SzczegolZamowienie.Szczegol.IdZamowienie,
                 IdBilet = x.SzczegolZamowienie.Szczegol.IdBilet,
                 Ilosc = x.SzczegolZamowienie.Szczegol.Ilosc,
                 Cena = x.SzczegolZamowienie.Szczegol.Cena,
                 NazwaBiletu = x.Bilet.Wydarzenie.Nazwa, 
                 DataWydarzenia = x.Bilet.DataWydarzenia,
                 ZdjecieUrl = x.Bilet.Wydarzenie.ZdjecieUrl,
                 Miejscowosc = x.Bilet.Lokalizacja.Miejscowosc,
                 NazwaLokalizacji = x.Bilet.Lokalizacja.NazwaMiejsca
             })
             .ToList();

            ViewBag.bilety = zamowienieSzczegolyList;
            return View(zamowienieSzczegolyList);
        }

        public IActionResult StatystykaSprzedazy(string Filtruj)
        {

            var statystyka = _context.ZamowienieSzczegoly.Include(u => u.Bilet).ToList();

            var wynik = (from zamowienieSzczegoly in _context.ZamowienieSzczegoly
                         join bilet in _context.Bilety on zamowienieSzczegoly.IdBilet equals bilet.Id
                         group zamowienieSzczegoly by new { bilet.Wydarzenie.Nazwa, bilet.Wydarzenie.ZdjecieUrl, bilet.Lokalizacja.Miejscowosc, bilet.Lokalizacja.NazwaMiejsca } into grupowaneBilety
                         select new
                         {
                             NazwaWydarzenia = grupowaneBilety.Key.Nazwa,
                             ZdjecieUrl = grupowaneBilety.Key.ZdjecieUrl, 
                             MiejsceWydarzenia = grupowaneBilety.Key.Miejscowosc,
                             NazwaMiejsca = grupowaneBilety.Key.NazwaMiejsca,
                             LacznaIlosc = grupowaneBilety.Sum(gb => gb.Ilosc),
                             Zarobek = grupowaneBilety.Sum(gb => gb.Cena)
                         })
               .OrderByDescending(v => v.LacznaIlosc)
               .ToList();
            ViewBag.SprzedazBiletow = wynik;
            return View();
        }


    }
}
