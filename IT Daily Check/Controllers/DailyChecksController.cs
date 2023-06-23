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
using Org.BouncyCastle.Asn1.Ocsp;
using static System.Net.Mime.MediaTypeNames;
using MimeKit;
using IT_Daily_Check.Settings;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace IT_Daily_Check.Controllers
{
    public class DailyChecksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly MailSettings _mailSettings;


        public DailyChecksController(ApplicationDbContext context, IHttpContextAccessor httpcontextAccessor, UserManager<User> userManager, IOptions<MailSettings> mailSettings)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
            _mailSettings = mailSettings.Value;
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

            string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Url.Action("Edit", "DailyChecks") + "/" + dailyCheck.Id;
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(user.Email);
            email.To.Add(MailboxAddress.Parse("francis.opogah@gmt-limited.com"));
            email.Subject = dailyCheck.Location == "Apapa" ? "DAILY CHECK" : "OFFDOCK AND BMS DAILY CHECK";           
            email.Body = new BodyBuilder { HtmlBody = string.Format("<P>Good Morning All</P> <br /><h3 style='color:black;'>Click on the link below to view details of the daily check <br />{0}</h3>", url) }.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return RedirectToAction("Index");
        }
        

        public async Task<IActionResult> Edit(int id)
        {

            ViewBag.ISPs = _context.internetServiceProviders.ToList();
            ViewBag.DeviceServices = _context.DeviceServices.ToList();
            ViewBag.CCTVs = _context.CCTVs.ToList();
            ViewBag.statuses = _context.Results.ToList();
            ViewBag.Locations = _context.Locations.ToList();

            // Retrieve the DailyCheck and its associated DeviceChecks from the database
            var dailyCheck = await _context.DailyChecks.Include(dc => dc.DeviceServicechecks)
                                                        .Include(c => c.CCTVchecks)
                                                        .Include(i => i.InternetServiceSpeedchecks)
                                                        .FirstOrDefaultAsync(dc => dc.Id == id);

            if (dailyCheck == null)
            {
                return NotFound();
            }

            // Map the DailyCheck and DeviceChecks to the view model
            var viewModel = new DailyCheckViewModel
            {
                Id = dailyCheck.Id,
                Name = dailyCheck.Name,
                Location = dailyCheck.Location,
                Created_By = dailyCheck.Created_By,
                Date_Created = dailyCheck.Date_Created,
                DeviceServicecheckViewModels = dailyCheck.DeviceServicechecks.Select(deviceCheck => new DeviceServicecheckViewModel
                {
                    id = deviceCheck.Id,
                    DeviceName = deviceCheck.DeviceName,
                    Status = deviceCheck.Status
                }).ToList(),
                InternetServiceSpeedcheckViewModels = dailyCheck.InternetServiceSpeedchecks.Select(internetService => new InternetServiceSpeedcheckViewModel
                {
                    Id = internetService.Id,
                    ISP_NAME = internetService.ISP_NAME,
                    DownloadSpeed = internetService.DownloadSpeed,
                    UploadSpeed = internetService.UploadSpeed
                }).ToList(),
                CCTVcheckViewModels = dailyCheck.CCTVchecks.Select(cctvCheck => new CCTVcheckViewModel 
                { 
                    Id = cctvCheck.Id,
                    Description = cctvCheck.Description,
                    Reasons = cctvCheck.Reasons,
                    Results = cctvCheck.Results,
                    Comments = cctvCheck.Comments
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: DailyChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date_Created,Location,Created_By")] DailyCheck dailyCheck)
        //{
        //    if (id != dailyCheck.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(dailyCheck);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DailyCheckExists(dailyCheck.Id))
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
        //    return View(dailyCheck);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DailyCheckViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the existing DailyCheck from the database
                var dailyCheck = await _context.DailyChecks.Include(dc => dc.DeviceServicechecks)
                                                            .Include(i => i.InternetServiceSpeedchecks)
                                                            .Include(c => c.CCTVchecks)
                                                            .FirstOrDefaultAsync(dc => dc.Id == id);

                if (dailyCheck == null)
                {
                    return NotFound();
                }

                // Update the properties of the DailyCheck
                dailyCheck.Location = viewModel.Location;

                List<InternetServiceSpeedcheckViewModel> internetServiceSpeedcheckViewModel = viewModel.InternetServiceSpeedcheckViewModels;
                if (internetServiceSpeedcheckViewModel != null)
                {
                    ICollection<InternetServiceSpeedcheck> internetServiceSpeedchecks = internetServiceSpeedcheckViewModel
                        .GroupBy(dc => dc.ISP_NAME)
                        .Select(group => group.First())
                        .Select(dc => new InternetServiceSpeedcheck
                        {
                            Id = dc.Id,
                            ISP_NAME = dc.ISP_NAME,
                            DownloadSpeed = dc.DownloadSpeed,
                            UploadSpeed = dc.UploadSpeed
                        }).ToList();


                    // Update the DeviceChecks by comparing the submitted DeviceChecks with the existing ones
                    UpdateInternetServiceChecks(dailyCheck.InternetServiceSpeedchecks, internetServiceSpeedchecks);
                }

                // Map Device Service Check Data being retrived from Frontedn
                List<DeviceServicecheckViewModel> deviceChecksViewModel = viewModel.DeviceServicecheckViewModels;
                if (deviceChecksViewModel != null)
                {
                    ICollection<DeviceServicecheck> deviceChecks = deviceChecksViewModel
                        .GroupBy(dc => dc.DeviceName)
                        .Select(group => group.First())
                        .Select(dc => new DeviceServicecheck
                        {
                            Id = dc.id,
                            DeviceName = dc.DeviceName,
                            Status = dc.Status
                        }).ToList();


                    // Update the DeviceChecks by comparing the submitted DeviceChecks with the existing ones
                    UpdateDeviceChecks(dailyCheck.DeviceServicechecks, deviceChecks);
                }

                
                    List<CCTVcheckViewModel> cCTVcheckViewModel = viewModel.CCTVcheckViewModels;
                if (cCTVcheckViewModel != null)
                {
                    ICollection<CCTVcheck> cCTVchecks = cCTVcheckViewModel
                        .GroupBy(dc => dc.Description)
                        .Select(group => group.First())
                        .Select(dc => new CCTVcheck
                        {
                            Id = dc.Id,
                            Description = dc.Description,
                            Results = dc.Results,
                            Reasons = dc.Reasons,
                            Comments = dc.Comments
                        }).ToList();


                    // Update the DeviceChecks by comparing the submitted DeviceChecks with the existing ones
                    UpdateCctvServiceChecks(dailyCheck.CCTVchecks, cCTVchecks);
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                // ...
                throw;
            }

            return View(viewModel);
        }

        private void UpdateDeviceChecks(ICollection<DeviceServicecheck> existingDeviceChecks, ICollection<DeviceServicecheck> submittedDeviceChecks)
        {
            // Remove any existing DeviceChecks that are not present in the submitted DeviceChecks
            existingDeviceChecks.Clear();

            // Update or add the submitted DeviceChecks
            foreach (var submittedDeviceCheck in submittedDeviceChecks)
            {
                existingDeviceChecks.Add(submittedDeviceCheck);
            }
        }

        private void UpdateInternetServiceChecks(ICollection<InternetServiceSpeedcheck> existingInternetSpeedChecks, ICollection<InternetServiceSpeedcheck> submittedInternetSpeedChecks)
        {
            // Remove any existing DeviceChecks that are not present in the submitted DeviceChecks
            existingInternetSpeedChecks.Clear();

            // Update or add the submitted DeviceChecks
            foreach (var submittedInternetSpeedCheck in submittedInternetSpeedChecks)
            {
                existingInternetSpeedChecks.Add(submittedInternetSpeedCheck);
            }
        }

        private void UpdateCctvServiceChecks(ICollection<CCTVcheck> existingCctvChecks, ICollection<CCTVcheck> submittedCctvChecks)
        {
            // Remove any existing DeviceChecks that are not present in the submitted DeviceChecks
            existingCctvChecks.Clear();

            // Update or add the submitted DeviceChecks
            foreach (var submittedCctvCheck in submittedCctvChecks)
            {
                existingCctvChecks.Add(submittedCctvCheck);
            }
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
