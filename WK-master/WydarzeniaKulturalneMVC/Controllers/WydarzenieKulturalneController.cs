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
    public class WydarzenieKulturalneController : Controller
    {
        private readonly WydarzeniaKulturalneContext _context;
        public WydarzenieKulturalneController(WydarzeniaKulturalneContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index(string Filtruj)
        {

            var wydarzenia = _context.WydarzenieKulturalne
              
                .Include(w => w.KategoriaWydarzenia)
                .ToList();

            ViewBag.Filtruj = Filtruj;

            if (Filtruj != null)
            {
                wydarzenia = wydarzenia.Where(f => ContainsString(f.Nazwa, Filtruj) ||
                                                
                                                 ContainsString(f.KategoriaWydarzenia.Nazwa, Filtruj))
                                      .ToList();
            }


            return View(wydarzenia);

        }
        // GET: WydarzenieKulturalne/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WydarzenieKulturalne == null)
            {
                return NotFound();
            }

            var wydarzenieKulturalne = await _context.WydarzenieKulturalne
                .Include(w => w.KategoriaWydarzenia)
            
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wydarzenieKulturalne == null)
            {
                return NotFound();
            }

            return View(wydarzenieKulturalne);
        }
        // GET: WydarzenieKulturalne/Details/5
        public async Task<IActionResult> DetailsCard(int? id)
        {

            if (id == null || _context.WydarzenieKulturalne == null)
            {
                return NotFound();
            }

            var wydarzenieKulturalne = await _context.WydarzenieKulturalne.Include(w => w.KategoriaWydarzenia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wydarzenieKulturalne == null)
            {
                return NotFound();
            }

            return View(wydarzenieKulturalne);
        }

        // GET: WydarzenieKulturalne/Create
        public IActionResult Create()
        {
            ViewBag.KategoriaWydarzenia = new SelectList(_context.KategoriaWydarzenia, "id", "Nazwa");
            ViewBag.SpecjalneTagi = new SelectList(_context.SpecjalnyTag, "Id", "Nazwa");
            return View();
        }

       
        [HttpPost]
    
        public async Task<IActionResult> Create( WydarzenieKulturalne wydarzenieKulturalne)
        {

                    wydarzenieKulturalne.DataUwtorzenia = DateTime.Now;
                    _context.Add(wydarzenieKulturalne);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
      
            // Jeśli ModelState.IsValid == false lub wystąpił inny błąd, pobierz ponownie listę kategorii i zwróć widok
            //ViewBag.Kategorie = _context.KategoriaWydarzenia.ToList();
            //ViewBag.SpecjalneTagi = _context.SpecjalnyTag.ToList();
            //ViewBag.KategoriaWydarzenia = new SelectList(_context.KategoriaWydarzenia, "id", "Nazwa");
            //ViewBag.SpecjalneTagi = new SelectList(_context.SpecjalnyTag, "Id", "Nazwa");
            //return View(wydarzenieKulturalne);
        }

        public async Task<IActionResult> Filtruj(string Filtruj)
        {

            var wydarzenia = _context.WydarzenieKulturalne
             
                .Include(w => w.KategoriaWydarzenia)
                .ToList();

            ViewBag.Filtruj = Filtruj;

            if (Filtruj != null)
            {
                wydarzenia = wydarzenia.Where(f => ContainsString(f.Nazwa, Filtruj) ||
                                              
                                                 ContainsString(f.KategoriaWydarzenia.Nazwa, Filtruj))
                                      .ToList();
            }
            return View(wydarzenia);

        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WydarzenieKulturalne == null)
            {
                return NotFound();
            }

            var wydarzenieKulturalne = await _context.WydarzenieKulturalne.FindAsync(id);
            if (wydarzenieKulturalne == null)
            {
                return NotFound();
            }
            ViewBag.KategoriaWydarzenia = new SelectList(_context.KategoriaWydarzenia, "id", "Nazwa");
            ViewBag.SpecjalneTagi = new SelectList(_context.SpecjalnyTag, "Id", "Nazwa");

            return View(wydarzenieKulturalne);
        }

        //public async Task<IActionResult> Edit(int id, WydarzenieKulturalne wydarzenieKulturalne)
        //{
        //    if (id != wydarzenieKulturalne.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {


        //            _context.Update(wydarzenieKulturalne);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!WydarzenieKulturalneExists(wydarzenieKulturalne.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.KategoriaWydarzenia = new SelectList(_context.KategoriaWydarzenia, "id", "Nazwa");
        //    ViewBag.SpecjalneTagi = new SelectList(_context.SpecjalnyTag, "Id", "Nazwa");

        //    return View(wydarzenieKulturalne);
        //}

        // GET: WydarzenieKulturalne/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WydarzenieKulturalne == null)
            {
                return NotFound();
            }

            var wydarzenieKulturalne = await _context.WydarzenieKulturalne
                .Include(w => w.KategoriaWydarzenia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wydarzenieKulturalne == null)
            {
                return NotFound();
            }

            return View(wydarzenieKulturalne);
        }

        // POST: WydarzenieKulturalne/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WydarzenieKulturalne == null)
            {
                return Problem("Entity set 'WydarzeniaKulturalneMVCContext.WydarzenieKulturalne'  is null.");
            }
            var wydarzenieKulturalne = await _context.WydarzenieKulturalne.FindAsync(id);
            if (wydarzenieKulturalne != null)
            {
                _context.WydarzenieKulturalne.Remove(wydarzenieKulturalne);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WydarzenieKulturalneExists(int id)
        {


            return (_context.WydarzenieKulturalne?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public bool ContainsString(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }
}
