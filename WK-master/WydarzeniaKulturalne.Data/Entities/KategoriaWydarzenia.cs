using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class KategoriaWydarzenia
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Nazwa { get; set; }
        public string? Opis { get; set; }
        public virtual ICollection<WydarzenieKulturalne>? WydarzenieKulturalne { get; set; }
    }
}