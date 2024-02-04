using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalneMVC.Models
{
    public class DaneKoszyk
    {
        public List<ElementKoszyka> ElementyKoszyka { get; set; }
        public decimal Razem { get; set; }
        public int IloscBiletow { get; set; }
    }
}
