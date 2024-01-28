using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalneMVC.Models
{
    public class Koszyk
    {
        private readonly WydarzeniaKulturalneContext _context;
        private string IdSesjiKoszyka;
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
        public void DodajDoKoszyka(Wydarzenie wydarzenie)
        {
            //Najpierw sprawdzamy czy w koszyku tego użytkownika jest ten towar
            var elementKoszyka = _context.ElementKoszyka.Where(e => e.IdTowaru == wydarzenie.IdWydarzenia && e.IdSesjiKoszyka == IdSesjiKoszyka).FirstOrDefault();
            //jeżeli w koszyku danego użytkownika nie ma tego towaru, to dodajemy
            if (elementKoszyka == null)
            {
                elementKoszyka = new ElementKoszyka()
                {
                    IdSesjiKoszyka = this.IdSesjiKoszyka, //tu daje, że ten towar jest towarem tego użytkownika
                    IdTowaru = wydarzenie.IdWydarzenia,
                    Wydarzenie = _context.Wydarzenie.Find(wydarzenie.IdWydarzenia),
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
        public async Task<List<ElementKoszyka>> GetElementyKoszyka()
        {
            return await _context.ElementKoszyka.Where(e => e.IdSesjiKoszyka == IdSesjiKoszyka).Include(e => e.Wydarzenie).ToListAsync();
        }
        //Funkcja oblicza wartość koszyka danego użytkownika
        public async Task<decimal> GetRazem()
        {
            var suma =
                (
                    from element in _context.ElementKoszyka //dla każdego elementu koszyka
                    where element.IdSesjiKoszyka == this.IdSesjiKoszyka //ktory należy  do danego użytkownika
                    select (decimal?)element.Ilosc * element.Wydarzenie.Cena //mnoże ilość razy cena
                ).SumAsync(); //i sumuje
            return await suma ?? decimal.Zero;
        }
    }
}
}
