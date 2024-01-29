using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
using System.Security.Cryptography;
using System.Text;

namespace WydarzeniaKulturalneMVC.Controllers
{
    public class UzytkownikController : Controller
    {
        private readonly WydarzeniaKulturalneContext _context;
        public UzytkownikController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Filtruj)
        {

            var uzytkownik = _context.Uzytkownik.Include(u => u.Rola).ToList();

            ViewBag.Filtruj = Filtruj;

            if (Filtruj != null)
                uzytkownik = uzytkownik.Where(f => ContainsString(f.Imie, Filtruj) ||
                ContainsString(f.Nazwisko, Filtruj) ||
                ContainsString (f.Rola.Nazwa, Filtruj)               
                ).ToList();
       

            return View(uzytkownik);

        }
        public IActionResult Rejestracja()
        {
            return View("Rejestracja");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            // Zeruj ciasteczka sesji
            foreach (var cookieKey in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookieKey);
            }

            return Redirect("/");

        }
        public IActionResult Logowanie()
        {
            return View("Logowanie");
        }

        [HttpPost]
        public async Task<IActionResult> Logowanie(string Email, string Haslo)
        {
            // Check for empty email or password
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Haslo))
            {
                ViewBag.ErrorMessageLogin = "Email i hasło są wymagane.";
                return View("Logowanie");
            }

            var uzytkownik = await _context.Uzytkownik.Include(x => x.Rola).FirstOrDefaultAsync(x => x.Email == Email);
            var ctx = HttpContext.User;

            if (uzytkownik == null)
            {
                ViewBag.ErrorMessageLogin = "Użytkownik o podanym adresie email nie istnieje.";
                return View("Logowanie");
            }

            if (uzytkownik.Haslo == hashPassword(Haslo))
            {

                ClaimsPrincipal rezultat = new ClaimsPrincipal();

                var claims = new List<Claim>
                {
                   new Claim(ClaimTypes.Email, uzytkownik.Email),
                    new Claim(ClaimTypes.Name, uzytkownik.Imie),
                    new Claim(ClaimTypes.Role, uzytkownik.Rola.Nazwa)
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity)));

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(30),
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);


                if (uzytkownik.Rola?.Nazwa == "Admin")
                {
                    return RedirectToAction("AdminPanel", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessageLogin = "Mail lub hasło jest niepoprawne.";
                return View("Logowanie");
            }
        }
    
        

        // GET: Uzytkownik/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Uzytkownik == null)
            {
                return NotFound();
            }

            var uzytkownik = await _context.Uzytkownik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uzytkownik == null)
            {
                return NotFound();
            }
          
            return View(uzytkownik);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Uzytkownik uzytkownik, string weryfikujHaslo)
        {
            // Check for empty email or password
            if (string.IsNullOrWhiteSpace(uzytkownik.Email) || string.IsNullOrWhiteSpace(uzytkownik.Haslo))
            {
                ViewBag.EmailErrorMessage = "Podaj adres email i hasło. Pole nie może być puste.";
                return View("Rejestracja");
            }
            if (string.IsNullOrWhiteSpace(uzytkownik.Imie) || string.IsNullOrWhiteSpace(uzytkownik.Nazwisko))
            {
                ViewBag.DaneErrorMessage = "Pole nie może być puste.";
                return View("Rejestracja");
            }

            // Check for white spaces in email or password
            if (uzytkownik.Email.Contains(" ") || uzytkownik.Haslo.Contains(" "))
            {
                ViewBag.EmailErrorMessage = "Adres email i hasło nie mogą zawierać spacji.";
                return View("Rejestracja");
            }

            var nowyUzytkownik = await _context.Uzytkownik.FirstOrDefaultAsync(u => u.Email == uzytkownik.Email);
            if (nowyUzytkownik != null)
            {
                ViewBag.EmailErrorMessage = "Email w użyciu. Proszę wybrać inny.";
                return View("Rejestracja");
            }

            if (hashPassword(uzytkownik.Haslo) == hashPassword(weryfikujHaslo))
            {
                if (nowyUzytkownik == null)
                {
                    if (ModelState.IsValid)
                    {
                        uzytkownik.Haslo = hashPassword(uzytkownik.Haslo);
                        _context.Add(uzytkownik);

                        await _context.SaveChangesAsync();
                        ViewBag.LoginMessage = "Konto zostało utworzone.";
                        TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
                        return View("Logowanie");
                    }

                    return View("Rejestracja");
                }
            }

            ViewBag.PasswordErrorMessage = "Podane hasła różnią się od siebie.";
            return View("Rejestracja");
        }

        string hashPassword(string password)
        {
            var sha = SHA512.Create();
            var asByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = sha.ComputeHash(asByteArray);
            return Convert.ToBase64String(hashedPassword);
        }
        public bool ContainsString(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }
   

}

//Edycja danych uzyutkownika ?
//Edycja hasla uzytkownika/? 