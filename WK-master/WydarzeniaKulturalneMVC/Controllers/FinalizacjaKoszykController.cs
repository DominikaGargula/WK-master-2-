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

        public async Task<IActionResult> Index(string Filtruj)
        {

            var zamowienieSzczegolyQuery = _context.ZamowienieSzczegoly
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
                     Marza = x.Bilet.Marza,
                     NazwaBiletu = x.Bilet.Wydarzenie.Nazwa,
                     DataWydarzenia = x.Bilet.DataWydarzenia,
                     ZdjecieUrl = x.Bilet.Wydarzenie.ZdjecieUrl,
                     Miejscowosc = x.Bilet.Lokalizacja.Miejscowosc,
                     NazwaLokalizacji = x.Bilet.Lokalizacja.NazwaMiejsca
                 });

            if (!string.IsNullOrEmpty(Filtruj))
            {
                zamowienieSzczegolyQuery = zamowienieSzczegolyQuery
                    .Where(x => x.IdZamowienie.ToString().Equals(Filtruj));
            }

            var zamowienieSzczegolyList = await zamowienieSzczegolyQuery.ToListAsync();

            ViewBag.Filtruj = Filtruj;
            ViewBag.SprzedaneBilety = zamowienieSzczegolyList;

            return View(zamowienieSzczegolyList);
        }

        public IActionResult Platnosc()
        {
            var idSesjiKoszyka = _koszyk.IdSesjiKoszyka;
            decimal sumaZamowienia = _context.ElementKoszyka
                                  .Include(b => b.Bilety)
                                 .Where(e => e.IdSesjiKoszyka == idSesjiKoszyka)
                                 .Sum(item => (item.Bilety.Wydarzenie.Cena + ((item.Bilety.Wydarzenie.Cena * item.Bilety.Marza)) / 100) * item.Ilosc);

            ViewBag.SumaZamowienia = sumaZamowienia;

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
                                            .Include(b => b.Bilety)
                                           .Where(e => e.IdSesjiKoszyka == idSesjiKoszyka)
                                           .Sum(item => (item.Bilety.Wydarzenie.Cena + item.Bilety.Marza) * item.Ilosc);

            ViewBag.SumaZamowienia = sumaZamowienia;
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
                TempData["ErrorMessagePlatnosc"] = "Błędny kod promocji. Jedyny właściwy to: WSB";
                return View(order);
            }
            try
            {
                // Zapisz zamówienie
                _context.Zamowienie.Add(order);
                _context.SaveChanges();

                // Pobierz elementy koszyka dla zalogowanego użytkownika
                var zalogowanyUzytkownikKoszyk = _context.ElementKoszyka
                        .Where(c => c.IdSesjiKoszyka == idSesjiKoszyka)
                        .Include(c => c.Bilety)
                        .Include(c => c.Bilety.Wydarzenie)
                        .ToList();

                // Przypisz elementy koszyka do zamówienia
                foreach (var cartItem in zalogowanyUzytkownikKoszyk)
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

                _context.ElementKoszyka.RemoveRange(zalogowanyUzytkownikKoszyk);
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
            var userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
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
                 Marza = x.Bilet.Marza,
                 NazwaBiletu = x.Bilet.Wydarzenie.Nazwa,
                 DataWydarzenia = x.Bilet.DataWydarzenia,
                 ZdjecieUrl = x.Bilet.Wydarzenie.ZdjecieUrl,
                 Miejscowosc = x.Bilet.Lokalizacja.Miejscowosc,
                 NazwaLokalizacji = x.Bilet.Lokalizacja.NazwaMiejsca
             })
             .ToList();

            return View(zamowienieSzczegolyList);
        }

        public IActionResult StatystykaSprzedazy(DateTime? dataWydarzeniaOd, DateTime? dataWydarzeniaDo)
        {


            var wynik = (from zamowienieSzczegoly in _context.ZamowienieSzczegoly
                         join bilet in _context.Bilety on zamowienieSzczegoly.IdBilet equals bilet.Id
                         join zamowienie in _context.Zamowienie on zamowienieSzczegoly.IdZamowienie equals zamowienie.IdZamowienie
                         group zamowienieSzczegoly by new { bilet.Wydarzenie.Nazwa, bilet.Wydarzenie.ZdjecieUrl, bilet.Lokalizacja.Miejscowosc, bilet.Lokalizacja.NazwaMiejsca, bilet.Marza } into grupowaneBilety
                         select new
                         {
                             NazwaWydarzenia = grupowaneBilety.Key.Nazwa,
                             ZdjecieUrl = grupowaneBilety.Key.ZdjecieUrl,
                             MiejsceWydarzenia = grupowaneBilety.Key.Miejscowosc,
                             NazwaMiejsca = grupowaneBilety.Key.NazwaMiejsca,
                             LacznaIlosc = grupowaneBilety.Sum(gb => gb.Ilosc),
                             ZyskZMarzy = grupowaneBilety.Sum(gb => gb.Ilosc * gb.Cena * grupowaneBilety.Key.Marza / 100),
                             Zarobek = grupowaneBilety.Sum(gb => gb.Ilosc * (gb.Cena + ((gb.Cena * grupowaneBilety.Key.Marza) / 100)))
                         })
        .OrderByDescending(v => v.LacznaIlosc)
        .ToList();
            ViewBag.SprzedazBiletow = wynik;
            if (dataWydarzeniaOd.HasValue || dataWydarzeniaDo.HasValue)
            {
                var koniecDniaDataDo = dataWydarzeniaDo?.AddDays(1).AddTicks(-1);
                var wynikF = (from zamowienieSzczegoly in _context.ZamowienieSzczegoly
                              join bilet in _context.Bilety on zamowienieSzczegoly.IdBilet equals bilet.Id
                              join zamowienie in _context.Zamowienie on zamowienieSzczegoly.IdZamowienie equals zamowienie.IdZamowienie
                              where (!dataWydarzeniaOd.HasValue || zamowienie.OrderDate >= dataWydarzeniaOd.Value) &&
                            (!koniecDniaDataDo.HasValue || zamowienie.OrderDate <= koniecDniaDataDo.Value)
                              group zamowienieSzczegoly by new { bilet.Wydarzenie.Nazwa, bilet.Wydarzenie.ZdjecieUrl, bilet.Lokalizacja.Miejscowosc, bilet.Lokalizacja.NazwaMiejsca, bilet.Marza } into grupowaneBilety
                              select new
                              {
                                  NazwaWydarzenia = grupowaneBilety.Key.Nazwa,
                                  ZdjecieUrl = grupowaneBilety.Key.ZdjecieUrl,
                                  MiejsceWydarzenia = grupowaneBilety.Key.Miejscowosc,
                                  NazwaMiejsca = grupowaneBilety.Key.NazwaMiejsca,
                                  LacznaIlosc = grupowaneBilety.Sum(gb => gb.Ilosc),
                                  ZyskZMarzy = grupowaneBilety.Sum(gb => gb.Ilosc * gb.Cena * grupowaneBilety.Key.Marza / 100),
                                  Zarobek = grupowaneBilety.Sum(gb => gb.Ilosc * (gb.Cena + ((gb.Cena * grupowaneBilety.Key.Marza) / 100)))
                              })
            .OrderByDescending(v => v.LacznaIlosc)
            .ToList();
                ViewBag.SprzedazBiletow = wynikF;
                ViewBag.DataOd = dataWydarzeniaOd;
                ViewBag.DataDo = dataWydarzeniaDo;
                return View();
            }
            return View();
        }
    }
}
