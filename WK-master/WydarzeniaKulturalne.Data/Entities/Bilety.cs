using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class Bilety
    {

        public int Id { get; set; }

        // Klucz obcy do WydarzenieKulturalne
        public int WydarzenieKulturalneId { get; set; }
        public virtual WydarzenieKulturalne Wydarzenie { get; set; }

        // Klucz obcy do LokalizacjaWydarzenia
        public int LokalizacjaWydarzeniaId { get; set; }

        public virtual LokalizacjaWydarzenia Lokalizacja { get; set; }


        [Display(Name = "Pula biletów")]
        public int IloscBiletow { get; set; }


        [Display(Name = "Data wydarzenia")]
        public DateTime DataWydarzenia { get; set; }

        [Display(Name = "Czy dostępne")]
        public bool CzyDostepne { get; set; }
    }
}
