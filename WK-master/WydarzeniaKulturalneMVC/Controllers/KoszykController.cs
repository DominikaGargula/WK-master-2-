using Microsoft.AspNetCore.Mvc;
using WydarzeniaKulturalneMVC.ViewModel;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalneMVC.Models;


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
                Razem = await koszyk.GetRazem()
            };
            //przekazujemy dane do koszyka złozone z dwóch elementów do widoku
            return View(daneDoKoszyka);


        }
        //Funkcja kontrollera dodaje towar do koszyka
        //To jest akcja, która zostanie wywołana pod przyciskiem dodaj do koszyka

        public async Task<ActionResult> DodajDoKoszyka(int id)
        {

            Koszyk koszyk = new Koszyk(_context, this.HttpContext);
            //do koszyka dodaje towar o danym id
            koszyk.DodajDoKoszyka(await _context.Bilety.FindAsync(id));
            //po dodaniu towaru do koszyka, przechodzę do tego koszyka, czyli do index, który wyświetli koszyk
            return RedirectToAction("Index");//to jest przejście do widoku index

        }
        [HttpPost]
        //public ActionResult UsunZKoszyka(int id)
        //{
        //    // Remove the item from the cart
        //    var koszyk = ShoppingCart.GetCart(this.HttpContext);

        //    // Get the name of the album to display confirmation
        //    string albumName = storeDB.Carts
        //        .Single(item => item.RecordId == id).Album.Title;

        //    // Remove from cart
        //    int itemCount = cart.RemoveFromCart(id);

        //    // Display the confirmation message
        //    var results = new ShoppingCartRemoveViewModel
        //    {
        //        Message = Server.HtmlEncode(albumName) +
        //            " has been removed from your shopping cart.",
        //        CartTotal = cart.GetTotal(),
        //        CartCount = cart.GetCount(),
        //        ItemCount = itemCount,
        //        DeleteId = id
        //    };
        //    return Json(results);
        //}

        public ActionResult PodsumowanieKoszyka()
        {
            // Utwórz instancję klasy Koszyk, przekazując do konstruktora wymagane parametry
            var koszyk = new Koszyk(_context, this.HttpContext);

            ViewData["koszykIlosc"] = koszyk.GetIlosc();
            return PartialView("PodsumowanieKoszyka");
        }

        public ActionResult UsunZKoszyka(int id)
        {
            // Usuń element z koszyka
            var koszyk = new Koszyk(_context, this.HttpContext);

            // Pobierz nazwę biletu do wyświetlenia potwierdzenia
            string nazwaBiletu = _context.ElementKoszyka
                .Single(item => item.IdElementuKoszyka == id).Bilety.Wydarzenie.Nazwa; // Zakładam, że masz właściwość "Nazwa" w modelu Bilety

            // Usuń z koszyka
            int iloscElementowWKoszyku = koszyk.UsunZKoszyka(id);

            // Wyświetl komunikat potwierdzenia
            var wyniki = new DaneKoszykUsuwanie
            {
                Informacja = System.Web.HttpUtility.HtmlEncode(nazwaBiletu) +
                    " został usunięty z koszyka.",
                SumaKoszyka = koszyk.GetRazem().Result, // Odczekaj na asynchroniczną operację
                LiczKoszyk = koszyk.GetIlosc(),
                LiczbaElementow = iloscElementowWKoszyku,
                UsunId = id
            };
            return Json(wyniki);
        }
    }
}

