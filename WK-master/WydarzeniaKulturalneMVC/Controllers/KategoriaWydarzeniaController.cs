using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalneMVC.Controllers
{
 
    public class KategoriaWydarzeniaController : Controller
    {
       
        private readonly WydarzeniaKulturalneContext _context;

        public KategoriaWydarzeniaController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }

        // GET: KategoriaWydarzenia
        public async Task<IActionResult> Index()
        {
              return _context.KategoriaWydarzenia != null ? 
                          View(await _context.KategoriaWydarzenia.ToListAsync()) :
                          Problem("Entity set 'WydarzeniaKulturalneMVCContext.KategoriaWydarzenia'  is null.");
        }

        // GET: KategoriaWydarzenia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KategoriaWydarzenia == null)
            {
                return NotFound();
            }

            var kategoriaWydarzenia = await _context.KategoriaWydarzenia
                .FirstOrDefaultAsync(m => m.id == id);
            if (kategoriaWydarzenia == null)
            {
                return NotFound();
            }

            return View(kategoriaWydarzenia);
        }

        // GET: KategoriaWydarzenia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KategoriaWydarzenia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Nazwa,Opis")] KategoriaWydarzenia kategoriaWydarzenia)
        {
            if (ModelState.IsValid) //sprawdza poprawnosc wprowadzanych parametrow
            {
                _context.Add(kategoriaWydarzenia);
                await _context.SaveChangesAsync();
                TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
                return RedirectToAction(nameof(Index));
            }
            return View(kategoriaWydarzenia);
        }

        // GET: KategoriaWydarzenia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KategoriaWydarzenia == null)
            {
                return NotFound();
            }

            var kategoriaWydarzenia = await _context.KategoriaWydarzenia.FindAsync(id);
            if (kategoriaWydarzenia == null)
            {
                return NotFound();
            }
            return View(kategoriaWydarzenia);
        }

        // POST: KategoriaWydarzenia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Nazwa,Opis")] KategoriaWydarzenia kategoriaWydarzenia)
        {
            if (id != kategoriaWydarzenia.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategoriaWydarzenia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriaWydarzeniaExists(kategoriaWydarzenia.id))
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
            return View(kategoriaWydarzenia);
        }

        // GET: KategoriaWydarzenia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KategoriaWydarzenia == null)
            {
                return NotFound();
            }

            var kategoriaWydarzenia = await _context.KategoriaWydarzenia
                .FirstOrDefaultAsync(m => m.id == id);
            if (kategoriaWydarzenia == null)
            {
                return NotFound();
            }

            return View(kategoriaWydarzenia);
        }

        // POST: KategoriaWydarzenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KategoriaWydarzenia == null)
            {
                return Problem("Entity set 'WydarzeniaKulturalneMVCContext.KategoriaWydarzenia'  is null.");
            }
            var kategoriaWydarzenia = await _context.KategoriaWydarzenia.FindAsync(id);
            if (kategoriaWydarzenia != null)
            {
                _context.KategoriaWydarzenia.Remove(kategoriaWydarzenia);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KategoriaWydarzeniaExists(int id)
        {
          return (_context.KategoriaWydarzenia?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
