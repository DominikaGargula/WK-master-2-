using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalne.Data;
using Microsoft.AspNetCore.Authorization;

namespace WydarzeniaKulturalneMVC.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class LokalizacjaWydarzeniaController : Controller
    {
        
        private readonly WydarzeniaKulturalneContext _context;

        public LokalizacjaWydarzeniaController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }

        // GET: LokalizacjaWydarzenia
        public async Task<IActionResult> Index()
        {
              return _context.LokalizacjaWydarzenia != null ? 
                          View(await _context.LokalizacjaWydarzenia.ToListAsync()) :
                          Problem("Entity set 'WydarzeniaKulturalneMVCContext.LokalizacjaWydarzenia'  is null.");
        }

        // GET: LokalizacjaWydarzenia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            var lokalizacjaWydarzenia = await _context.LokalizacjaWydarzenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            return View(lokalizacjaWydarzenia);
        }

        // GET: LokalizacjaWydarzenia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LokalizacjaWydarzenia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Miejscowosc,KodPocztowy,Ulica,NumerDomu,NazwaMiejsca")] LokalizacjaWydarzenia lokalizacjaWydarzenia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lokalizacjaWydarzenia);
                await _context.SaveChangesAsync();
                TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
                return RedirectToAction(nameof(Index));
            }
            return View(lokalizacjaWydarzenia);
        }

        // GET: LokalizacjaWydarzenia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            var lokalizacjaWydarzenia = await _context.LokalizacjaWydarzenia.FindAsync(id);
            if (lokalizacjaWydarzenia == null)
            {
                return NotFound();
            }
            return View(lokalizacjaWydarzenia);
        }

        // POST: LokalizacjaWydarzenia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LokalizacjaWydarzenia lokalizacjaWydarzenia)
        {
            if (id != lokalizacjaWydarzenia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lokalizacjaWydarzenia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LokalizacjaWydarzeniaExists(lokalizacjaWydarzenia.Id))
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
            return View(lokalizacjaWydarzenia);
        }

        // GET: LokalizacjaWydarzenia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            var lokalizacjaWydarzenia = await _context.LokalizacjaWydarzenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lokalizacjaWydarzenia == null)
            {
                return NotFound();
            }

            return View(lokalizacjaWydarzenia);
        }

        // POST: LokalizacjaWydarzenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LokalizacjaWydarzenia == null)
            {
                return Problem("Entity set 'WydarzeniaKulturalneMVCContext.LokalizacjaWydarzenia'  is null.");
            }
            var lokalizacjaWydarzenia = await _context.LokalizacjaWydarzenia.FindAsync(id);
            if (lokalizacjaWydarzenia != null)
            {
                _context.LokalizacjaWydarzenia.Remove(lokalizacjaWydarzenia);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LokalizacjaWydarzeniaExists(int id)
        {
          return (_context.LokalizacjaWydarzenia?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
