using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class Uzytkownik
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Imię")]
        public string? Imie { get; set; }
        [Required]
        [MaxLength(150)]
        public string? Nazwisko { get; set; }
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)\\S{8,20}$", ErrorMessage = "Hasło musi zawierać co najmniej jedną: dużą i małą literę, cyfrę oraz mieć długość 8-20 znaków")]
        [Display(Name = "Hasło")]
        //[PasswordValidatorAttribute]
        public string? Haslo { get; set; } 
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{1,6}$", ErrorMessage = "Nieprawidłowy adres email")]
        public string? Email { get; set; }
        public int? RolaId {get; set; } = 2;
        public Rola? Rola { get; set; }

    }

}
