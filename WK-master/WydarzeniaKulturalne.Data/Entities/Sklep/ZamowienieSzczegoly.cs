using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities.Sklep
{
    public class ZamowienieSzczegoly //OrderDetail
    {
        public int IdZamowienieSzczegoly { get; set; } //OrderDetailId
        public int IdZamowienie { get; set; } //OrderDetail
        public int IdBilet { get; set; }
        public int Ilosc { get; set; } //Quantity
        public decimal Cena { get; set; } //UnitPrice
        public virtual Bilety Bilety { get; set; } //Album Album
        public virtual Zamowienie Zamowienie { get; set; } //Order Order
    }
}
