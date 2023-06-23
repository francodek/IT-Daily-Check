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
using IT_Daily_Check.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Xml.Linq;

namespace IT_Daily_Check.Controllers
{
    public class DailyChecksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<User> _userManager;


        public DailyChecksController(ApplicationDbContext context, IHttpContextAccessor httpcontextAccessor, UserManager<User> userManager = null)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
        }

        // GET: DailyChecks
        public async Task<IActionResult> Index()
        {
              return View(await _context.DailyChecks.ToListAsync());
        }

        // GET: DailyChecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DailyChecks == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyCheck == null)
            {
                return NotFound();
            }

            return View(dailyCheck);
        }

        // GET: DailyChecks/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.ISPs = _context.internetServiceProviders.ToList();
            ViewBag.DeviceServices = _context.DeviceServices.ToList();
            ViewBag.CCTVs = _context.CCTVs.ToList();
            ViewBag.statuses = _context.Results.ToList();
            ViewBag.Locations = _context.Locations.ToList();
            return View();
        }

        // POST: DailyChecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyCheckViewModel model)
        {
            var userId = _httpcontextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var firstName = user.FirstName;

            var lastName = user.LastName;
            var dailyCheck = new DailyCheck
            {
                Name = $"Daily Check - {model.Location} - {DateTime.Now}",
                Location = model.Location,
                Date_Created = DateTime.Now,
                Created_By = $"{firstName} {lastName}"
            };

            // Create Internet Service Checks 

            if(model.InternetServiceSpeedcheckViewModels != null && model.InternetServiceSpeedcheckViewModels.Any())
            {
                dailyCheck.InternetServiceSpeedchecks = model.InternetServiceSpeedcheckViewModels
                    .GroupBy(i => i.ISP_NAME)
                    .Select(i => i.First())
                    .Select(ispcheck => new InternetServiceSpeedcheck
                    {
                        ISP_NAME = ispcheck.ISP_NAME,
                        DownloadSpeed = ispcheck.DownloadSpeed,
                        UploadSpeed = ispcheck.UploadSpeed
                    }).ToList();
            }

            // Create Device/Service Checks
            if(model.DeviceServicecheckViewModels != null && model.DeviceServicecheckViewModels.Any())
            {
                dailyCheck.DeviceServicechecks = model.DeviceServicecheckViewModels
                    .GroupBy(i => i.DeviceName)
                    .Select(i => i.First())
                    .Select(deviceCheck => new DeviceServicecheck
                    {
                        DeviceName = deviceCheck.DeviceName,
                        Status = deviceCheck.Status
                    }).ToList();
            }

            // Create CCTV Checkks
            if (model.CCTVcheckViewModels != null && model.CCTVcheckViewModels.Any())
            {
                dailyCheck.CCTVchecks = model.CCTVcheckViewModels
                    .GroupBy(i => i.Description)
                    .Select(i => i.First())
                    .Select(CCTVcheck => new CCTVcheck
                    {
                        Description = CCTVcheck.Description,
                        Results = CCTVcheck.Results,
                        Reasons = CCTVcheck.Reasons,
                        Comments = CCTVcheck.Comments
                    }).ToList();
            }

            // Save the DailyCheck along with the associated checks
            _context.DailyChecks.Add(dailyCheck);
            _context.SaveChanges();
            return RedirectToAction("Index");   
        }

        // GET: DailyChecks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DailyChecks == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks.FindAsync(id);
            if (dailyCheck == null)
            {
                return NotFound();
            }
            return View(dailyCheck);
        }

        // POST: DailyChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date_Created,Location,Created_By")] DailyCheck dailyCheck)
        {
            if (id != dailyCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyCheck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyCheckExists(dailyCheck.Id))
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
            return View(dailyCheck);
        }

        // GET: DailyChecks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DailyChecks == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyCheck == null)
            {
                return NotFound();
            }

            return View(dailyCheck);
        }

        // POST: DailyChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DailyChecks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DailyChecks'  is null.");
            }
            var dailyCheck = await _context.DailyChecks.FindAsync(id);
            if (dailyCheck != null)
            {
                _context.DailyChecks.Remove(dailyCheck);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyCheckExists(int id)
        {
          return _context.DailyChecks.Any(e => e.Id == id);
        }
    }
}
