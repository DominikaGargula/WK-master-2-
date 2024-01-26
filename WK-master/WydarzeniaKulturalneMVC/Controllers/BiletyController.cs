using WydarzeniaKulturalne.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalne.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WydarzeniaKulturalneMVC.Controllers
{
    public class BiletyController : Controller
    {
        private readonly WydarzeniaKulturalneContext _context;
        public BiletyController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }
        public IActionResult Index(string Filtruj)
        {
            var bilety = _context.Bilety
             .Include(w => w.Wydarzenie)
             .Include(w => w.Lokalizacja)
             .ToList();

            ViewBag.Filtruj = Filtruj;
            if (Filtruj != null)
            {
                bilety = bilety.Where(f => ContainsString(f.Wydarzenie.Nazwa, Filtruj) ||
                ContainsString(f.Lokalizacja.NazwaMiejsca, Filtruj))
                    .ToList();
            }

            return View(bilety);
        }
        public async Task<IActionResult> DetailsCard(int? id)
        {
            if (id == null || _context.Bilety == null)
            {
                return NotFound();
            }
            var bilety = await _context.Bilety.Include(w => w.Lokalizacja).Include(w => w.Wydarzenie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bilety == null)
            {
                return NotFound();
            }
            return View(bilety);
        }
        public IActionResult Create()
        {
            ViewBag.LokalizacjaWydarzenia = new SelectList(_context.LokalizacjaWydarzenia, "Id", "NazwaMiejsca");
            ViewBag.WydarzenieKulturalne = new SelectList(_context.WydarzenieKulturalne, "Id", "Nazwa");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bilety bilety)
        {

            if (ModelState.IsValid) //sprawdza poprawnosc wprowadzanych parametrow
            {
                _context.Add(bilety);
                await _context.SaveChangesAsync();
                TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LokalizacjaWydarzenia = new SelectList(_context.LokalizacjaWydarzenia, "Id", "NazwaMiejsca", bilety.LokalizacjaWydarzeniaId);
            ViewBag.WydarzenieKulturalne = new SelectList(_context.WydarzenieKulturalne, "Id", "Nazwa", bilety.WydarzenieKulturalneId);

            return View(bilety);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bilety == null)
            {
                return NotFound();
            }
            var bilety = await _context.Bilety.FindAsync(id);
            if (bilety == null)
            {
                return NotFound();
            }
            ViewBag.LokalizacjaWydarzenia = new SelectList(_context.LokalizacjaWydarzenia, "Id", "NazwaMiejsca");
            ViewBag.WydarzenieKulturalne = new SelectList(_context.WydarzenieKulturalne, "Id", "Nazwa");

            return View(bilety);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bilety bilety)
        {
            if (id != bilety.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bilety);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BiletyExists(bilety.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LokalizacjaWydarzenia = new SelectList(_context.LokalizacjaWydarzenia, "Id", "NazwaMiejsca");
            ViewBag.WydarzenieKulturalne = new SelectList(_context.WydarzenieKulturalne, "Id", "Nazwa");

            return View(bilety);
        }
        private bool BiletyExists(int id)
        {
            return (_context.Bilety?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            var bilety = await _context.Bilety
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bilety == null)
            {
                return NotFound();
            }

            return View(bilety);
        }

        // POST: LokalizacjaWydarzenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bilety == null)
            {
                return Problem("Entity set 'WydarzeniaKulturalneMVCContext.Bilety'  is null.");
            }
            var bilet = await _context.Bilety.FindAsync(id);
            if (bilet != null)
            {
                _context.Bilety.Remove(bilet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public bool ContainsString(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }
}
