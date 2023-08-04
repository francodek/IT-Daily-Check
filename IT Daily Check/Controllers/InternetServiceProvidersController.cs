using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IT_Daily_Check.Data;
using IT_Daily_Check.Models;
using Microsoft.AspNetCore.Authorization;

namespace IT_Daily_Check.Controllers
{
    [Authorize]
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
              return View(await _context.InternetServiceProviders.ToListAsync());
        }

        // GET: InternetServiceProviders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InternetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.InternetServiceProviders
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
            if (id == null || _context.InternetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.InternetServiceProviders.FindAsync(id);
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
            if (id == null || _context.InternetServiceProviders == null)
            {
                return NotFound();
            }

            var internetServiceProvider = await _context.InternetServiceProviders
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
            if (_context.InternetServiceProviders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.internetServiceProviders'  is null.");
            }
            var internetServiceProvider = await _context.InternetServiceProviders.FindAsync(id);
            if (internetServiceProvider != null)
            {
                _context.InternetServiceProviders.Remove(internetServiceProvider);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InternetServiceProviderExists(int id)
        {
          return _context.InternetServiceProviders.Any(e => e.Id == id);
        }
    }
}
