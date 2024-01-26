using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WydarzeniaKulturalne.Data;
using WydarzeniaKulturalne.Data.Entities;
using WydarzeniaKulturalneMVC.Models;

namespace WydarzeniaKulturalneMVC.Controllers;

public class RolaController : Controller
{

    private readonly ILogger<HomeController> _logger;


    private readonly WydarzeniaKulturalneContext _context;

    public RolaController(WydarzeniaKulturalneContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Rola.ToListAsync());
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
    public async Task<IActionResult> Create(Rola rola)
    {
        if (ModelState.IsValid)
        {
            _context.Add(rola);
            await _context.SaveChangesAsync();
            TempData["Save"] = "Pomyślnie utworzono nowy obiekt";
            return RedirectToAction(nameof(Index));
        }
        return View(rola);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Rola == null)
        {
            return NotFound();
        }

        var rola = await _context.Rola.FindAsync(id);
        if (rola == null)
        {
            return NotFound();
        }
        return View(rola);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rola role)
    {
        if (id != role.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(role);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(role.Id))
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
        return View(role);
    }

    // GET: LokalizacjaWydarzenia/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Rola == null)
        {
            return NotFound();
        }

        var role = await _context.Rola
            .FirstOrDefaultAsync(m => m.Id == id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: LokalizacjaWydarzenia/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Rola == null)
        {
            return Problem("Entity set 'WydarzeniaKulturalneMVCContext.Rola'  is null.");
        }
        var role = await _context.Rola.FindAsync(id);
        if (role != null)
        {
            _context.Rola.Remove(role);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    private bool RoleExists(int id)
    {
        return (_context.Rola?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
