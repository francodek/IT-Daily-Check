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
    public class CCTVsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CCTVsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CCTVs
        public async Task<IActionResult> Index()
        {
              return View(await _context.CCTVs.ToListAsync());
        }

        // GET: CCTVs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CCTVs == null)
            {
                return NotFound();
            }

            var cCTV = await _context.CCTVs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cCTV == null)
            {
                return NotFound();
            }

            return View(cCTV);
        }

        // GET: CCTVs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CCTVs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Location")] CCTV cCTV)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cCTV);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cCTV);
        }

        // GET: CCTVs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CCTVs == null)
            {
                return NotFound();
            }

            var cCTV = await _context.CCTVs.FindAsync(id);
            if (cCTV == null)
            {
                return NotFound();
            }
            return View(cCTV);
        }

        // POST: CCTVs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Location")] CCTV cCTV)
        {
            if (id != cCTV.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cCTV);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CCTVExists(cCTV.Id))
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
            return View(cCTV);
        }

        // GET: CCTVs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CCTVs == null)
            {
                return NotFound();
            }

            var cCTV = await _context.CCTVs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cCTV == null)
            {
                return NotFound();
            }

            return View(cCTV);
        }

        // POST: CCTVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CCTVs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CCTVs'  is null.");
            }
            var cCTV = await _context.CCTVs.FindAsync(id);
            if (cCTV != null)
            {
                _context.CCTVs.Remove(cCTV);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CCTVExists(int id)
        {
          return _context.CCTVs.Any(e => e.Id == id);
        }
    }
}
