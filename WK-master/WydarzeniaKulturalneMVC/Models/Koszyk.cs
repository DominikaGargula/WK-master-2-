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


        public string GetIdSesjiKoszyka(HttpContext httpContext)
        {

            var idSesjiKoszyka = httpContext.Session.GetString("IdSesjiKoszyka");

            if (string.IsNullOrWhiteSpace(idSesjiKoszyka))
            {
                Guid tempIdSesjiKoszyka = Guid.NewGuid();

                idSesjiKoszyka = tempIdSesjiKoszyka.ToString();
                httpContext.Session.SetString("IdSesjiKoszyka", idSesjiKoszyka);
            }

            return idSesjiKoszyka;
        }

        public void DodajDoKoszyka(Bilety bilety)
        {

            var elementKoszyka = _context.ElementKoszyka
                .Where(e => e.IdBilet == bilety.Id && e.IdSesjiKoszyka == IdSesjiKoszyka)
                .FirstOrDefault();

            if (elementKoszyka == null)
            {
                elementKoszyka = new ElementKoszyka()
                {
                    IdSesjiKoszyka = this.IdSesjiKoszyka,
                    IdBilet = bilety.Id,
                    Bilety = _context.Bilety.Find(bilety.Id),
                    Ilosc = 1,
                    DataUtworzenia = DateTime.Now

                };
                _context.ElementKoszyka.Add(elementKoszyka);

            }
            else
            {
                elementKoszyka.Ilosc++;
            }
            _context.SaveChanges();
        }

        public int UsunZKoszyka(int idElementuKoszyka)
        {
            var elementKoszyka = _context.ElementKoszyka
                .Where(e => e.IdElementuKoszyka == idElementuKoszyka && e.IdSesjiKoszyka == IdSesjiKoszyka)
                .FirstOrDefault();
            int liczbaElementowWKoszyku = 0;

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
                _context.SaveChanges();
            }

            return liczbaElementowWKoszyku;
        }


        public List<ElementKoszyka> GetElementyKoszyka()

        {
            return _context.ElementKoszyka
                .Where(e => e.IdSesjiKoszyka == IdSesjiKoszyka)
                .Include(e => e.Bilety)
                .Include(w => w.Bilety.Lokalizacja)
                .Include(b => b.Bilety.Wydarzenie)
                .ToList();
        }

        public async Task<decimal> GetRazem()//łączna wartość koszyka
        {
            var suma = await (
                from element in _context.ElementKoszyka
                where element.IdSesjiKoszyka == this.IdSesjiKoszyka
                select (decimal?)element.Ilosc * (element.Bilety.Wydarzenie.Cena + ((element.Bilety.Wydarzenie.Cena * element.Bilety.Marza) / 100))
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
        //public int GetIlosc() 
        //{

        //    int? zlicz = (from element in _context.ElementKoszyka
        //                  where element.IdSesjiKoszyka == this.IdSesjiKoszyka
        //                  select (int?)element.Ilosc).Sum();
        //    return zlicz ?? 0;
        //}
        //public int StworzZamowienie(Zamowienie zamowienie)
        //{
        //    decimal wartoscZamowienia = 0;
        //    var elementyKoszyka = GetElementyKoszyka();
        //    foreach (var item in elementyKoszyka)
        //    {
        //        var szczegolyZamowienia = new ZamowienieSzczegoly
        //        {
        //            IdBilet = item.IdBilet,
        //            IdZamowienie = zamowienie.IdZamowienie,
        //            Cena = item.Bilety.Wydarzenie.Cena,
        //            Ilosc = item.Ilosc
        //        };

        //        wartoscZamowienia += (item.Ilosc * item.Bilety.Wydarzenie.Cena);
        //        zamowienie.ZamowienieSzczegolu.Add(szczegolyZamowienia);
        //    }
        //    zamowienie.Suma = wartoscZamowienia;
        //    zamowienie.OrderDate = DateTime.Now;
        //    _context.Zamowienie.Add(zamowienie);
        //    _context.SaveChanges();
        //    OproznijKoszyk();
        //    return zamowienie.IdZamowienie;
        //}

        //public void OproznijKoszyk()
        //{
        //    var elementyKoszyka = _context.ElementKoszyka.Where(
        //        e => e.IdSesjiKoszyka == IdSesjiKoszyka).ToList();

        //    foreach (var element in elementyKoszyka)
        //    {
        //        _context.ElementKoszyka.Remove(element);
        //    }

        //    _context.SaveChanges();
        //}
        public void MigrujKoszyk(string nazwaUzytkownika)
        {


            var koszyk = _context.ElementKoszyka.Where(c => c.IdSesjiKoszyka == IdSesjiKoszyka).ToList();
            foreach (ElementKoszyka item in koszyk)
            {
                item.IdSesjiKoszyka = this.IdSesjiKoszyka;
            }
            _context.SaveChanges();
        }

    }
}

