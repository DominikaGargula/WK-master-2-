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
using WydarzeniaKulturalneMVC.Models;

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

            return Redirect("/");

        }
        public IActionResult Logowanie()
        {
            return View("Logowanie");
        }
        [HttpPost]
        public async Task<IActionResult> Logowanie(string Email, string Haslo)
        {
           
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Haslo))
            {
                ViewBag.ErrorMessageLogin = "Email i hasło są wymagane.";
                return View("Logowanie");
            }

            var uzytkownik = await _context.Uzytkownik.Include(x => x.Rola).FirstOrDefaultAsync(x => x.Email == Email);

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
            new Claim(ClaimTypes.Name, uzytkownik.Email),
            new Claim(ClaimTypes.Role, uzytkownik.Rola.Nazwa),
            new Claim(ClaimTypes.NameIdentifier, uzytkownik.Id.ToString()),
        };

                var koszyk = new Koszyk(_context, this.HttpContext);

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
  
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(30),
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity)));
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

            if (hashPassword(uzytkownik.Haslo) != hashPassword(weryfikujHaslo))
            {
                ViewBag.PasswordErrorMessage = "Hasła różnią się od siebie. Zweryfikuj wpisane dane";
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


                        ViewBag.LoginMessage = "Konto zostało pomyślnie utworzone";
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uzytkownik = await _context.Uzytkownik.FindAsync(id);
            if (uzytkownik == null)
            {
                return NotFound();
            }
            return View(uzytkownik);
        }

        // UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Imie,Nazwisko,Haslo,Email,RolaId")] Uzytkownik uzytkownik)
        {
            if (id != uzytkownik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(uzytkownik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uzytkownik);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
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

        // DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(id);
            _context.Uzytkownik.Remove(uzytkownik);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
   

}

//Edycja danych uzyutkownika ?
//Edycja hasla uzytkownika/? 