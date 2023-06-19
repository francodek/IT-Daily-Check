using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IT_Daily_Check.Data;
using IT_Daily_Check.Models;

namespace IT_Daily_Check.Controllers
{
    public class InternetServiceProvidersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InternetServiceProvidersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InternetServiceProviders
        public async Task<IActionResult> Index()
        {
              return View(await _context.internetServiceProviders.ToListAsync());
        }

        // GET: InternetServiceProviders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.internetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.internetServiceProviders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (internetServiceProvider == null)
            {
                return NotFound();
            }

            return View(internetServiceProvider);
        }

        // GET: InternetServiceProviders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InternetServiceProviders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] InternetServiceProvider internetServiceProvider)
        {
            if (ModelState.IsValid)
            {
                _context.Add(internetServiceProvider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(internetServiceProvider);
        }

        // GET: InternetServiceProviders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.internetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.internetServiceProviders.FindAsync(id);
            if (internetServiceProvider == null)
            {
                return NotFound();
            }
            return View(internetServiceProvider);
        }

        // POST: InternetServiceProviders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] InternetServiceProvider internetServiceProvider)
        {
            if (id != internetServiceProvider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(internetServiceProvider);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InternetServiceProviderExists(internetServiceProvider.Id))
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
            return View(internetServiceProvider);
        }

        // GET: InternetServiceProviders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.internetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.internetServiceProviders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (internetServiceProvider == null)
            {
                return NotFound();
            }

            return View(internetServiceProvider);
        }

        // POST: InternetServiceProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.internetServiceProviders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.internetServiceProviders'  is null.");
            }
            var internetServiceProvider = await _context.internetServiceProviders.FindAsync(id);
            if (internetServiceProvider != null)
            {
                _context.internetServiceProviders.Remove(internetServiceProvider);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InternetServiceProviderExists(int id)
        {
          return _context.internetServiceProviders.Any(e => e.Id == id);
        }
    }
}
