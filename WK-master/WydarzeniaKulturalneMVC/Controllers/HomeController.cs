using System.Diagnostics;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalneMVC.Models;

namespace WydarzeniaKulturalneMVC.Controllers;

public class HomeController : Controller
{

    private readonly ILogger<HomeController> _logger;
    private readonly WydarzeniaKulturalneContext _context;

    public HomeController(ILogger<HomeController> logger, WydarzeniaKulturalneContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {

        var wydarzenie = _context.WydarzenieKulturalne.Include(w => w.KategoriaWydarzenia).
            ToList();
        var bilety = _context.Bilety
            .Include(w => w.Lokalizacja)
            .Include(w => w.Wydarzenie)
            .ToList();

        ViewBag.Wydarzenie = bilety; //promowane

        ViewBag.NoweWydarzenia = bilety.OrderByDescending(x => x.Id).
           Take(5).ToList();

        ViewBag.LokalizacjaWydarzenia = _context.Bilety
                   .Select(b => b.Lokalizacja)
                   .Where(l => l != null)  // Filtrowanie lokalizacji, które nie są null
                   .Distinct()
                   .ToList();

        ViewBag.NazwaKategorii = wydarzenie.FirstOrDefault()?.KategoriaWydarzenia?.Nazwa;

        var kategorieZWydarzeniami = _context.KategoriaWydarzenia
        .Where(k => k.WydarzenieKulturalne != null && k.WydarzenieKulturalne
        .Any(w => w.Bilety != null && w.Bilety.Any()))
    .ToList();

        ViewBag.KategoriaWydarzenia = kategorieZWydarzeniami;
        var wynik = (from zamowienieSzczegoly in _context.ZamowienieSzczegoly
                     join bilet in _context.Bilety on zamowienieSzczegoly.IdBilet equals bilet.Id
                     group zamowienieSzczegoly by new { bilet.Wydarzenie.Nazwa, bilet.Lokalizacja.Miejscowosc, bilet.Id } into grupowaneBilety
                     select new
                     {
                         Id = grupowaneBilety.Key.Id,
                         NazwaWydarzenia = grupowaneBilety.Key.Nazwa,
                         MiejsceWydarzenia = grupowaneBilety.Key.Miejscowosc,
                         LacznaIlosc = grupowaneBilety.Sum(gb => gb.Ilosc)
                     })
                    .OrderByDescending(v => v.LacznaIlosc)
                    .ToList();

        ViewBag.TopSprzedaz = wynik;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPanel()
    {

        var wydarzenia = _context.WydarzenieKulturalne

            .Include(x => x.KategoriaWydarzenia).ToList();

        var suma = _context.WydarzenieKulturalne.ToList();

        ViewBag.SumaBiletowSprzedanych = _context.ZamowienieSzczegoly.Sum(b => b.Ilosc);
        ViewBag.SumaBiletow = _context.Bilety.Sum(b => b.IloscBiletow);
        ViewBag.SumaWydarzen = _context.WydarzenieKulturalne.Count();

        ViewBag.SumaUzytkownikow = _context.Uzytkownik.Where(u => u.Rola.Nazwa != "Admin").Count();
        ViewBag.WydarzenieAktywne = _context.Bilety.Where(u => u.IloscBiletow > 0).Count();
        ViewBag.WydarzenieNieAktywne = _context.Bilety.Where(u => u.IloscBiletow == 0).Count();
        ViewBag.NowiUzytkownicy = _context.Uzytkownik.Where(u => u.Rola.Nazwa == "Uzytkownik").
                                                      OrderByDescending(u => u.Id).
                                                      Take(5).ToList();
        Random random = new Random();
        int LosoweId = random.Next(0, suma.Count);
        ViewBag.Random = wydarzenia[LosoweId];
        ViewBag.NoweWydarzenia = wydarzenia.OrderByDescending(x => x.Id).
            Take(5).ToList();
        ViewBag.Promowane = _context.Bilety.Where(p => p.Wydarzenie.Promowane == true)
            .OrderByDescending(p => p.Id).
            Take(5).ToList();
        ViewBag.Bilety = _context.Bilety
            .Include(w => w.Lokalizacja)
            .Include(w => w.Wydarzenie)
            .ToList();

        var wynik = (from zamowienieSzczegoly in _context.ZamowienieSzczegoly
                     join bilet in _context.Bilety on zamowienieSzczegoly.IdBilet equals bilet.Id
                     group zamowienieSzczegoly by new { bilet.Wydarzenie.Nazwa, bilet.Wydarzenie.ZdjecieUrl, bilet.Lokalizacja.Miejscowosc, bilet.Lokalizacja.NazwaMiejsca } into grupowaneBilety
                     select new
                     {


                         NazwaWydarzenia = grupowaneBilety.Key.Nazwa,
                         ZdjecieUrl = grupowaneBilety.Key.ZdjecieUrl,
                         MiejsceWydarzenia = grupowaneBilety.Key.Miejscowosc,
                         NazwaMiejsca = grupowaneBilety.Key.NazwaMiejsca,
                         LacznaIlosc = grupowaneBilety.Sum(gb => gb.Ilosc)
                     })
                .OrderByDescending(v => v.LacznaIlosc)
                .Take(5)
                .ToList();


        ViewBag.TopSprzedaz = wynik;

        return View();
    }
    public async Task<IActionResult> Filtruj(string Szukaj)

    {
        var kategorieZWydarzeniami = _context.KategoriaWydarzenia
        .Where(k => k.WydarzenieKulturalne != null && k.WydarzenieKulturalne
        .Any(w => w.Bilety != null && w.Bilety.Any()))
        .ToList();
        ViewBag.KategoriaWydarzenia = kategorieZWydarzeniami;

        ViewBag.LokalizacjaWydarzenia = await _context.Bilety
        .Select(b => b.Lokalizacja)
        .Where(l => l != null)
        .Distinct()
        .ToListAsync();

        var bilety = _context.Bilety
            .Include(x => x.Wydarzenie)
            .Include(x => x.Wydarzenie.KategoriaWydarzenia)
            .ToList();

        ViewBag.FiltrujListe = Szukaj;

        if (!string.IsNullOrWhiteSpace(Szukaj) && Szukaj.Length > 2)
        {
            if (Filtruj != null)
                bilety = bilety.Where(f => ContainsString(f.Wydarzenie.Nazwa, Szukaj) ||
                ContainsString(f.Wydarzenie.KategoriaWydarzenia.Nazwa, Szukaj) ||
                ContainsString(f.Lokalizacja.Miejscowosc, Szukaj) ||
                ContainsString(f.Lokalizacja.NazwaMiejsca, Szukaj)
                ).ToList();

            return View(bilety);
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public bool ContainsString(string source, string toCheck)
    {
        return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
    }
    [HttpGet]
    public async Task<IActionResult> ListaWydarzen(int? id)
    {

        var kategorieZWydarzeniami = _context.KategoriaWydarzenia
       .Where(k => k.WydarzenieKulturalne != null && k.WydarzenieKulturalne
       .Any(w => w.Bilety != null && w.Bilety.Any()))
       .ToList();
        ViewBag.KategoriaWydarzenia = kategorieZWydarzeniami;

        ViewBag.LokalizacjaWydarzenia = await _context.Bilety
              .Select(b => b.Lokalizacja)
              .Where(l => l != null)
              .Distinct()
              .ToListAsync();
        var bilety = await _context.Bilety
        .Include(w => w.Wydarzenie.KategoriaWydarzenia)
        .Where(item => item.Wydarzenie.KategoriaWydarzeniaId == id)
        .OrderBy(w => w.DataWydarzenia)
        .ToListAsync();

        return View(bilety);
    }

    public async Task<IActionResult> ListaLokalizacji(int? id)
    {

        var kategorieZWydarzeniami = _context.KategoriaWydarzenia
       .Where(k => k.WydarzenieKulturalne != null && k.WydarzenieKulturalne
       .Any(w => w.Bilety != null && w.Bilety.Any()))
       .ToList();
        ViewBag.KategoriaWydarzenia = kategorieZWydarzeniami;


        ViewBag.LokalizacjaWydarzenia = await _context.Bilety
        .Select(b => b.Lokalizacja)
        .Where(l => l != null)
        .Distinct()
        .ToListAsync();

        var bilety = await _context.Bilety
        .Include(w => w.Wydarzenie.KategoriaWydarzenia)
        .Where(item =>
            (id == null || item.LokalizacjaWydarzeniaId == id))
        .OrderBy(w => w.DataWydarzenia)
        .ToListAsync();

        return View(bilety);
    }


    [HttpPost]
    public IActionResult WyszukajPoDacie(DateTime dataWydarzenia)
    {


        var biletyZDaty = _context.Bilety
            .Where(b => b.DataWydarzenia.Date == dataWydarzenia.Date)
            .Include(b => b.Wydarzenie)
            .ToList();

        return View("WynikData", biletyZDaty);
    }

}
