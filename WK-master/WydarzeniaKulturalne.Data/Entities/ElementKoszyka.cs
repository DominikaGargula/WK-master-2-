using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{

    public class ElementKoszyka
    {
        [Key]
        public int IdElementuKoszyka { get; set; }
        public string IdSesjiKoszyka { get; set; }
        public int IdBilet { get; set; } //idtowaru
        public  virtual Bilety Bilety { get; set; } // public virtual Wydarzenie Wydarzenie 
        public int Ilosc { get; set; }
        public DateTime DataUtworzenia { get; set; }


    }
}

