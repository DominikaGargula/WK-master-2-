using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace WydarzeniaKulturalne.Data.Entities

{
    public class LokalizacjaWydarzenia
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Miejscowość jest wymagana")]
        [MaxLength(50)]
        [Display(Name = "Miejscowość")]
        public string? Miejscowosc { get; set; }
        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Nieprawidłowy format kodu pocztowego. Poprawny format to XX-XXX.")]
        [Display(Name = "Kod pocztowy")]
        public string KodPocztowy { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Ulica jest wymagana")]
        public string Ulica { get; set; }
        [Required(ErrorMessage = "Numer budynku jest wymagany")]
        [MaxLength(5)]
        [Display(Name = "Numer budynku")]
        public string NumerDomu { get; set; }
        [Required(ErrorMessage = "Nazwa miejsca jest wymagana")]
        [MinLength(3)]
        [Display(Name = "Nazwa miejsca")]
        public string NazwaMiejsca { get; set; }

        public virtual ICollection<WydarzenieKulturalne>? WydarzenieKulturalne { get; set; }
    }
}
