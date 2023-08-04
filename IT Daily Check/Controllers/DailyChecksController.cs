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
using MimeKit;
using IT_Daily_Check.Settings;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using RazorLight;
using IT_Daily_Check.Views.DailyChecks;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IT_Daily_Check.Controllers
{
    public class DailyChecksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;


        public DailyChecksController(ApplicationDbContext context, IHttpContextAccessor httpcontextAccessor, UserManager<User> userManager, IOptions<MailSettings> mailSettings, IWebHostEnvironment webHostEnvironment, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
            _mailSettings = mailSettings.Value;
            _webHostEnvironment = webHostEnvironment;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            
        }

        
        

            // GET: DailyChecks
        public async Task<IActionResult> Index(int pageNumber=1)
        {
            return View(await PaginatedList<DailyCheck>.CreateAsync(_context.DailyChecks,pageNumber,2));
        }

        public IActionResult NoCheckView()
        {
            return View();
        }

        public async Task<IActionResult> GetCurrentDayCheck()
        {
            var dailyCheck = await _context.DailyChecks.Where(x => x.Date_Created.Date == DateTime.Now.Date).ToListAsync();
            if (!dailyCheck.Any())
            {
                return RedirectToAction("NoCheckView");
            }
            return View(dailyCheck);
        }

        public async Task<IActionResult> GetDailyCheckForSelectedDate(DateTime date)
        {
            var dailyCheck = await _context.DailyChecks.Where(x => x.Date_Created.Date == date.Date).ToListAsync();
            if (!dailyCheck.Any())
            {
                TempData["Error"] = "No Check Created For Searched Date";
                return RedirectToAction("GetCurrentDayCheck");
            }
            return View(dailyCheck);
        }

        // GET: DailyChecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            ViewBag.ISPs = _context.InternetServiceProviders.ToList();
            ViewBag.DeviceServices = _context.DeviceServices.ToList();
            ViewBag.CCTVs = _context.CCTVs.ToList();
            ViewBag.statuses = _context.Results.ToList();
            ViewBag.Locations = _context.Locations.ToList();

            // Retrieve the DailyCheck and its associated DeviceChecks from the database
            var dailyCheck = await _context.DailyChecks.Include(dc => dc.DeviceServicechecks)
                                                        .Include(c => c.CCTVchecks)
                                                        .Include(i => i.InternetServiceSpeedchecks)
                                                        .FirstOrDefaultAsync(dc => dc.Id == id);
            
            var cctvs = dailyCheck.CCTVchecks.Select(x => x.Description).ToList();
            foreach (var cctv in dailyCheck.CCTVchecks)
            {
                var cctvLocation = await _context.CCTVs.Where(x => x.Description == cctv.Description).FirstOrDefaultAsync();

            }

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
                Comments = dailyCheck.Comments,
                Date_Created = dailyCheck.Date_Created,
                ImageOneName = dailyCheck.ImageOneName,
                ImageTwoName = dailyCheck.ImageTwoName,
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
                    Comments = cctvCheck.Comments,
                    Location =  GetCctvLocation(cctvCheck.Description),                    
                }).ToList()
            };

            return View(viewModel);
        }

        private string GetCctvLocation(string description)
        {
            var cctv =  _context.CCTVs.Where(x => x.Description == description).FirstOrDefault();
            return cctv.Location;
        }

        // GET: DailyChecks/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.ISPs = _context.InternetServiceProviders.ToList();
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
            var dailycheck = _context.DailyChecks.Where(x => x.Date_Created.Date == DateTime.Now.Date && x.Location == model.Location && x.Comments == model.Comments).FirstOrDefault();
            if (dailycheck != null)
            {
                TempData["Error"] = $"A Check has already been created for {model.Location}";
                return RedirectToAction("Index");
            }


            // Image Creation Section
            string imageOneName = "";
            if (model.ImageUploadOne != null)
            {
                string folder = "media/images/";
                imageOneName = Guid.NewGuid().ToString() + "_" + model.ImageUploadOne.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder, imageOneName);

                await model.ImageUploadOne.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            string imageTwoName = "";
            if (model.ImageUploadTwo != null)
            {
                string folder = "media/images/";
                imageTwoName = Guid.NewGuid().ToString() + "_" + model.ImageUploadTwo.FileName;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder, imageTwoName);

                await model.ImageUploadTwo.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            var userId = _httpcontextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var firstName = user.FirstName;

            var lastName = user.LastName;
            var dailyCheck = new DailyCheck
            {
                Name = $"Daily Check - {model.Location} - {DateTime.Now}",
                Location = model.Location,
                Date_Created = DateTime.Now,
                Created_By = $"{firstName} {lastName}",
                ImageOneName = imageOneName,
                ImageTwoName = imageTwoName,
                Comments = model.Comments
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
                        Comments = CCTVcheck.Comments,
                        Location = GetCctvLocation(CCTVcheck.Description),

                    }).ToList();
            }
            
            // Save the DailyCheck along with the associated checks
            _context.DailyChecks.Add(dailyCheck);
            _context.SaveChanges();

            string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Url.Action("Details", "DailyChecks") + "/" + dailyCheck.Id;            

            
            string viewHtml = await RenderViewToStringAsync("EmailTemplate", dailyCheck);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(user.Email);
            email.To.Add(MailboxAddress.Parse("francis.opogah@gmt-limited.com"));
            // email.To.Add(MailboxAddress.Parse("fisayoadegun@gmail.com"));
            email.Cc.Add(MailboxAddress.Parse("francisopogah45@gmail.com"));
           // email.To.Add(MailboxAddress.Parse("oluwadare.aborisade@gmt-limited.com"));
            email.Subject = dailyCheck.Location == "Apapa" ? "DAILY CHECK" : dailyCheck.Location == "Abule-Oshun" 
                ? "OFFDOCK AND BMS DAILY CHECK" : "DAILY CHECK";            
            var bodyBuilder = new BodyBuilder();

            if (model.ImageUploadOne != null)
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    model.ImageUploadOne.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                bodyBuilder.Attachments.Add(dailyCheck.ImageOneName, fileBytes, ContentType.Parse(model.ImageUploadOne.ContentType));
            }

            if (model.ImageUploadTwo != null)
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    model.ImageUploadTwo.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                bodyBuilder.Attachments.Add(dailyCheck.ImageTwoName, fileBytes, ContentType.Parse(model.ImageUploadTwo.ContentType));
            }
            
            bodyBuilder.HtmlBody = viewHtml.Replace("[url]", url);
            
            email.Body = bodyBuilder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return RedirectToAction("GetCurrentDayCheck");
        }

        private async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            string assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var metadataReference = Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(Path.Combine(assemblyFolder, "IT Daily check.dll"));
            var engine = new RazorLightEngineBuilder()
             .UseMemoryCachingProvider()
             .UseEmbeddedResourcesProject(typeof(EmailTemplateModel))
             .EnableDebugMode(true)
             .AddMetadataReferences(metadataReference)
             .Build();
            string viewHtml = await engine.CompileRenderAsync<object>(viewName, model);            

            return viewHtml;
        }


        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {

            ViewBag.ISPs = _context.InternetServiceProviders.ToList();
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
                Comments = dailyCheck.Comments,
                Date_Created = dailyCheck.Date_Created,
                ImageOneName = dailyCheck.ImageOneName,
                ImageTwoName = dailyCheck.ImageTwoName,
                ImageUploadOne = dailyCheck.ImageUploadOne,
                ImageUploadTwo = dailyCheck.ImageUploadTwo,
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


        [Authorize]
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

                if (viewModel.ImageUploadOne != null)
                {                    
                    string imageOneuploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/images/");
                    if (!string.IsNullOrEmpty(viewModel.ImageOneName))
                    {
                        string oldImagePath = Path.Combine(imageOneuploadsDir, viewModel.ImageOneName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    string imageOneName = Guid.NewGuid().ToString() + "_" + viewModel.ImageUploadOne.FileName;
                    string filePath1 = Path.Combine(imageOneuploadsDir, imageOneName);
                    FileStream fs1 = new FileStream(filePath1, FileMode.Create);
                    await viewModel.ImageUploadOne.CopyToAsync(fs1);
                    fs1.Close();
                    dailyCheck.ImageOneName = imageOneName;
                }



                if (viewModel.ImageUploadTwo != null)
                {                    
                    string imageTwouploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/images/");
                    if (!string.IsNullOrEmpty(viewModel.ImageTwoName))
                    {
                        string oldImagePath = Path.Combine(imageTwouploadsDir, viewModel.ImageTwoName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    string imageTwoName = Guid.NewGuid().ToString() + "_" + viewModel.ImageUploadTwo.FileName;
                    string filePath2 = Path.Combine(imageTwouploadsDir, imageTwoName);
                    FileStream fs2 = new FileStream(filePath2, FileMode.Create);
                    await viewModel.ImageUploadTwo.CopyToAsync(fs2);
                    fs2.Close();
                    dailyCheck.ImageTwoName = imageTwoName;
                }




                if (dailyCheck == null)
                {
                    return NotFound();
                }

                // Update the properties of the DailyCheck
                dailyCheck.Location = viewModel.Location;
                dailyCheck.Comments = viewModel.Comments;
                dailyCheck.ImageUploadOne = dailyCheck.ImageUploadOne;
                dailyCheck.ImageUploadTwo = dailyCheck.ImageUploadTwo;               

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
        [Authorize]
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
                if (dailyCheck.ImageOneName != null)
                {                                        
                    string imageOneuploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/images/");
                    if (!string.IsNullOrEmpty(dailyCheck.ImageOneName))
                    {
                        string oldImagePath = Path.Combine(imageOneuploadsDir, dailyCheck.ImageOneName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }


                if (dailyCheck.ImageTwoName != null)
                {                    
                    string imageTwouploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/images/");
                    if (!string.IsNullOrEmpty(dailyCheck.ImageTwoName))
                    {
                        string oldImagePath = Path.Combine(imageTwouploadsDir, dailyCheck.ImageTwoName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
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
