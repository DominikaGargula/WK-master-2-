using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalneMVC.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<WydarzenieKulturalne> WydarzeniaI { get; set; }
        public IEnumerable<KategoriaWydarzenia> KategorieI { get; set; }
        public IEnumerable<LokalizacjaWydarzenia> LokalizacjaI { get; set; }
    }
}
