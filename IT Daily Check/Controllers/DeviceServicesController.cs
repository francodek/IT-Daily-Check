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
    public class DeviceServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeviceServices
        public async Task<IActionResult> Index()
        {
              return View(await _context.DeviceServices.ToListAsync());
        }

        // GET: DeviceServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DeviceServices == null)
            {
                return NotFound();
            }

            var deviceService = await _context.DeviceServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceService == null)
            {
                return NotFound();
            }

            return View(deviceService);
        }

        // GET: DeviceServices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeviceServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] DeviceService deviceService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deviceService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deviceService);
        }

        // GET: DeviceServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DeviceServices == null)
            {
                return NotFound();
            }

            var deviceService = await _context.DeviceServices.FindAsync(id);
            if (deviceService == null)
            {
                return NotFound();
            }
            return View(deviceService);
        }

        // POST: DeviceServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] DeviceService deviceService)
        {
            if (id != deviceService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deviceService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceServiceExists(deviceService.Id))
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
            return View(deviceService);
        }

        // GET: DeviceServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DeviceServices == null)
            {
                return NotFound();
            }

            var deviceService = await _context.DeviceServices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deviceService == null)
            {
                return NotFound();
            }

            return View(deviceService);
        }

        // POST: DeviceServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DeviceServices == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DeviceServices'  is null.");
            }
            var deviceService = await _context.DeviceServices.FindAsync(id);
            if (deviceService != null)
            {
                _context.DeviceServices.Remove(deviceService);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceServiceExists(int id)
        {
          return _context.DeviceServices.Any(e => e.Id == id);
        }
    }
}
