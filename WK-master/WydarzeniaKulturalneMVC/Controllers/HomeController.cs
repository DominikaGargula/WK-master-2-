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


    //to jest obiekt reprezentującyych
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

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    //[Authorize(Roles = "Admin")]
    public IActionResult AdminPanel()
    {

        var wydarzenia = _context.WydarzenieKulturalne

            .Include(x => x.KategoriaWydarzenia).ToList();

        var suma = _context.WydarzenieKulturalne.ToList();

        ViewBag.SumaBiletow = _context.Bilety.Sum(b => b.IloscBiletow);
        ViewBag.SumaWydarzen = _context.WydarzenieKulturalne.Count();
        ViewBag.LiczbaKategorii = _context.KategoriaWydarzenia.Count();
        ViewBag.SumaUzytkownikow = _context.Uzytkownik.Where(u => u.Rola.Nazwa != "Admin").Count();

        ViewBag.NowiUzytkownicy = _context.Uzytkownik.Where(u => u.Rola.Nazwa == "Uzytkownik").
                                                      OrderByDescending(u => u.Id).
                                                      Take(5).ToList();
        Random random = new Random();
        int LosoweId = random.Next(0, suma.Count);
        ViewBag.Random = wydarzenia[LosoweId];
        ViewBag.NoweWydarzenia = wydarzenia.OrderByDescending(x => x.Id).
            Take(5).ToList();
        ViewBag.Promowane = _context.WydarzenieKulturalne.Where(p => p.Promowane == true)
            .OrderByDescending(p => p.Id).
            Take(5).ToList();

        return View();
    }


    //public async Task<IActionResult> Filtruj(string Szukaj)

    //{
    //    var wydarzenia = _context.WydarzenieKulturalne.Include(x => x.KategoriaWydarzenia).ToList();
    //    ViewBag.FiltrujListe = Szukaj;

    //    if (!string.IsNullOrWhiteSpace(Szukaj) && Szukaj.Length > 2)
    //    {
    //        if (Filtruj != null)
    //            wydarzenia = wydarzenia.Where(f => ContainsString(f.Nazwa, Szukaj) ||
    //            ContainsString(f.KategoriaWydarzenia.Nazwa, Szukaj)
    //            ).ToList();

    //        return View(wydarzenia);
    //    }

    //    return View();
    //}
    public async Task<IActionResult> Filtruj(string Szukaj)

    {
        var kategorieZWydarzeniami = _context.KategoriaWydarzenia
        .Where(k => k.WydarzenieKulturalne != null && k.WydarzenieKulturalne
        .Any(w => w.Bilety != null && w.Bilety.Any()))
        .ToList();
        ViewBag.KategoriaWydarzenia = kategorieZWydarzeniami;

        ViewBag.LokalizacjaWydarzenia = await _context.Bilety
        .Select(b => b.Lokalizacja)
        .Where(l => l != null)  // Filtrowanie lokalizacji, które nie są null
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
                ContainsString(f.Wydarzenie.KategoriaWydarzenia.Nazwa, Szukaj)
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
              .Where(l => l != null)  // Filtrowanie lokalizacji, które nie są null
              .Distinct()
              .ToListAsync();
        var bilety = await _context.Bilety.Include(w => w.Wydarzenie.KategoriaWydarzenia)
      .Where(item => item.Wydarzenie.KategoriaWydarzeniaId == id)
      .ToListAsync();



        //ViewBag.Nazwa = bilety;

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
        .Where(l => l != null)  // Filtrowanie lokalizacji, które nie są null
        .Distinct()
        .ToListAsync();

        var bilety = await _context.Bilety
        .Include(w => w.Wydarzenie.KategoriaWydarzenia)
        .Where(item =>
            (id == null || item.LokalizacjaWydarzeniaId == id))
        .ToListAsync();


        //ViewBag.Nazwa = bilety;

        return View(bilety);
    }
    [HttpPost]
    public IActionResult WyszukajPoDacie(DateTime dataWydarzenia)
    {
        // Tutaj możesz użyć dataWydarzenia do porównania z datami w bazie danych
        // Przykładowe zapytanie LINQ
        var biletyZDaty = _context.Bilety
            .Where(b => b.DataWydarzenia.Date == dataWydarzenia.Date)
            .ToList();

        return View("WynikiWyszukiwania", biletyZDaty);
    }

}
