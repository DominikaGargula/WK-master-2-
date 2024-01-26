using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalneMVC.Models;

namespace WydarzeniaKulturalneMVC.Controllers
{
    public class SpecjalnyTagController : Controller
    {
        private readonly WydarzeniaKulturalneContext _context;

        public SpecjalnyTagController(WydarzeniaKulturalneContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SpecjalnyTag.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecjalnyTag specjalnyTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specjalnyTag);
                await _context.SaveChangesAsync();
                TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
                return RedirectToAction(nameof(Index));
            }
            return View(specjalnyTag);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SpecjalnyTag == null)
            {
                return NotFound();
            }

            var specjalnyTag = await _context.SpecjalnyTag.FindAsync(id);
            if (specjalnyTag == null)
            {
                return NotFound();
            }
            return View(specjalnyTag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecjalnyTag specjalnyTag)
        {
            if (id != specjalnyTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specjalnyTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(specjalnyTag.Id))
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
            return View(specjalnyTag);
        }

        // GET: LokalizacjaWydarzenia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SpecjalnyTag == null)
            {
                return NotFound();
            }

            var specjalnyTag = await _context.SpecjalnyTag
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specjalnyTag == null)
            {
                return NotFound();
            }

            return View(specjalnyTag);
        }

        // POST: LokalizacjaWydarzenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SpecjalnyTag == null)
            {
                return Problem("Entity set 'WydarzeniaKulturalneMVCContext.specjalnyTag'  is null.");
            }
            var specjalnyTag = await _context.SpecjalnyTag.FindAsync(id);
            if (specjalnyTag != null)
            {
                _context.SpecjalnyTag.Remove(specjalnyTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool RoleExists(int id)
        {
            return (_context.SpecjalnyTag?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}