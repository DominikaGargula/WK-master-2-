using Humanizer;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalneMVC.Models
{
    public class Koszyk
    {
        private readonly WydarzeniaKulturalneContext _context;
        public string IdSesjiKoszyka;
        public Koszyk(WydarzeniaKulturalneContext context, HttpContext httpContext)
        {
            _context = context;
            IdSesjiKoszyka = GetIdSesjiKoszyka(httpContext);
        }
        // to jest funkcja która zwraca idSesji danej przeglądarki
        // to jest funkcja która zwraca idSesji danej przeglądarki
        private string GetIdSesjiKoszyka(HttpContext httpContext)
        {
            //jeżeli idSesjiKoszyka jest nullem
            if (httpContext.Session.GetString("IdSesjiKoszyka") == null)
            {
                //jeżeli identity użytkownika nie jest puste i nie posiada białych znaków
                if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.Name))
                {
                    //Wtedy staje się idSesja koszyka
                    httpContext.Session.SetString("IdSesjiKoszyka", httpContext.User.Identity.Name); //tutaj szukaj rozwiazania 
                }
                else
                {
                    //w przeciwnym wypadku generujemy idSesjiKoszyka przy pomocy Guid

                    Guid tempIdSesjiKoszyka = Guid.NewGuid();
                    httpContext.Session.SetString("IdSesjiKoszyka", tempIdSesjiKoszyka.ToString());
                }
            }
            return httpContext.Session.GetString("IdSesjiKoszyka").ToString();
        }
        //to jest funkcja ktora dodaje nowy towar danego uzytkownika do koszyja
        public void DodajDoKoszyka(Bilety bilety)
        {
            //Najpierw sprawdzamy czy w koszyku tego użytkownika jest ten towar
            var elementKoszyka = _context.ElementKoszyka
                .Where(e => e.IdBilet == bilety.Id && e.IdSesjiKoszyka == IdSesjiKoszyka)
                .FirstOrDefault();
            //jeżeli w koszyku danego użytkownika nie ma tego towaru, to dodajemy
            if (elementKoszyka == null)
            {
                elementKoszyka = new ElementKoszyka()
                {
                    IdSesjiKoszyka = this.IdSesjiKoszyka, //tu daje, że ten towar jest towarem tego użytkownika
                    IdBilet = bilety.Id,
                    Bilety = _context.Bilety.Find(bilety.Id),
                    Ilosc = 1,
                    DataUtworzenia = DateTime.Now

                };
                //dodajemy nowy element do kolekcji.
                _context.ElementKoszyka.Add(elementKoszyka);

            }
            else
            {
                //jeżeli dany towar jest w kosyku danego użytkownika, to zwiększamy jego ilość o 1
                elementKoszyka.Ilosc++;
            }
            //zapisujemy zmiany w bazie danych
            _context.SaveChanges();
        }
        // to jest metoda, która pobiera elementy koszyka danego użytkownika

        public int UsunZKoszyka(int idElementuKoszyka)
        {
            // Znajdź element koszyka dla danego biletu
            var elementKoszyka = _context.ElementKoszyka
                .Where(e => e.IdElementuKoszyka == idElementuKoszyka && e.IdSesjiKoszyka == IdSesjiKoszyka)
                .FirstOrDefault();
            int liczbaElementowWKoszyku = 0;

            // Jeżeli element koszyka istnieje, zmniejsz jego ilość o 1 lub usuń go, jeśli ilość spadnie do zera
            if (elementKoszyka != null)
            {
                if (elementKoszyka.Ilosc > 1)
                {
                    elementKoszyka.Ilosc--;
                    liczbaElementowWKoszyku = elementKoszyka.Ilosc;
                }
                else
                {
                    _context.ElementKoszyka.Remove(elementKoszyka);
                }
                // Zapisz zmiany w bazie danych
                _context.SaveChanges();
            }
           
            return liczbaElementowWKoszyku;
        }

        public void OproznijKoszyk()
        {
            var elementyKoszyka = _context.ElementKoszyka.Where(
                e => e.IdSesjiKoszyka == IdSesjiKoszyka).ToList(); //dodanie tolist

            foreach (var element in elementyKoszyka)
            {
                _context.ElementKoszyka.Remove(element);
            }
            //_context.ElementKoszyka.RemoveRange(elementyKoszyka);
            // Save changes
            _context.SaveChanges();
        }

        // Pobiera wszystkie elementy koszyka dla danego identyfikatora koszyka (idsesjikoszyka)
        // i zamień je na listę (ToList()).
        public List<ElementKoszyka> GetElementyKoszyka()

        {
            return _context.ElementKoszyka
                .Where(e => e.IdSesjiKoszyka == IdSesjiKoszyka)
                .Include(e => e.Bilety)
                .Include(w => w.Bilety.Lokalizacja)
                .Include(b => b.Bilety.Wydarzenie)
                .ToList();
        }
        //Funkcja oblicza wartość koszyka danego użytkownika
        //public async Task<decimal> GetRazem()
        //{
        //    var suma =
        //        (
        //            from element in _context.ElementKoszyka //dla każdego elementu koszyka
        //            where element.IdSesjiKoszyka == this.IdSesjiKoszyka //ktory należy  do danego użytkownika
        //            select (decimal?)element.Ilosc * element.Bilety.Wydarzenie.Cena //mnoże ilość razy cena
        //        ).SumAsync(); //i sumuje
        //    return await suma ?? decimal.Zero;
        //}
        public async Task<decimal> GetRazem()//łączna wartość koszyka
        {
            var suma = await (
                from element in _context.ElementKoszyka
                where element.IdSesjiKoszyka == this.IdSesjiKoszyka
                select (decimal?)element.Ilosc * element.Bilety.Wydarzenie.Cena
            ).SumAsync() ?? decimal.Zero;

            return suma;
        }
        public async Task<int> GetIloscBiletow()
        {
            var iloscBiletow = await (
                from element in _context.ElementKoszyka
                where element.IdSesjiKoszyka == this.IdSesjiKoszyka
                select (int?)element.Ilosc
            ).SumAsync() ?? 0;

            return iloscBiletow;
        }
        public int GetIlosc() // służy do pobierania liczby przedmiotów znajdujących się w koszyku
        {

            int? zlicz = (from element in _context.ElementKoszyka
                          where element.IdSesjiKoszyka == this.IdSesjiKoszyka
                          select (int?)element.Ilosc).Sum();
            //jeśli zlicz nie jest null, to zwróci jego wartość; w przeciwnym razie zwróci 0.
            return zlicz ?? 0;
        }
        public int StworzZamowienie(Zamowienie zamowienie)
        {
            decimal wartoscZamowienia = 0;

            var elementyKoszyka = GetElementyKoszyka();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in elementyKoszyka)
            {
                var szczegolyZamowienia = new ZamowienieSzczegoly
                {
                    IdBilet = item.IdBilet, // Zakładam, że masz identyfikator biletu
                    IdZamowienie = zamowienie.IdZamowienie,
                    Cena = item.Bilety.Wydarzenie.Cena, // Zakładam, że masz cenę biletu w modelu Bilet
                    Ilosc = item.Ilosc
                };
                // Set the order total of the shopping cart
                wartoscZamowienia += (item.Ilosc * item.Bilety.Wydarzenie.Cena);

                zamowienie.ZamowienieSzczegolu.Add(szczegolyZamowienia);
            }

            // Set the order's total to the orderTotal count
            zamowienie.Suma = wartoscZamowienia;

            // Set the order date
            zamowienie.OrderDate = DateTime.Now;

            // Save the order
            _context.Zamowienie.Add(zamowienie);
            _context.SaveChanges();

            // Empty the shopping cart
            OproznijKoszyk();

            // Return the OrderId as the confirmation number
            return zamowienie.IdZamowienie;
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrujKoszyk(string nazwaUzytkownika)
        {
            var koszyk = _context.ElementKoszyka.Where(
                c => c.IdSesjiKoszyka == IdSesjiKoszyka);

            foreach (ElementKoszyka item in koszyk)
            {
                item.IdSesjiKoszyka = nazwaUzytkownika;
            }
            _context.SaveChanges();
        }

    }
}

