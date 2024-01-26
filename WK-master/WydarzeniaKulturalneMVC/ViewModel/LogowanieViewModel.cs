using System.ComponentModel.DataAnnotations;

namespace WydarzeniaKulturalneMVC.ViewModel
{
    public class LogowanieViewModel
    {
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{1,6}$", ErrorMessage = "Nieprawidłowy adres email")]
        public string? Email { get; set; }
        public string? Haslo { get; set; }
    }
}
