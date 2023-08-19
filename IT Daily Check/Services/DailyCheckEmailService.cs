using IT_Daily_Check.Models;
using ITDailyCheck.Services.Interfaces;
using IT_Daily_Check.Settings;
using IT_Daily_Check.Views.DailyChecks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using RazorLight;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Hosting;

namespace ITDailyCheck.Services
{
    public class DailyCheckEmailService : IDailyCheckEmailService
    {        
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DailyCheckEmailService(IHttpContextAccessor httpcontextAccessor, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task SendDailyCheckToEmail(DailyCheck dailyCheck, string toEmail, string url, string host, int port, string mail, string password)
        {           
            var userId = _httpcontextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            string viewHtml = await RenderViewToStringAsync("EmailTemplate", dailyCheck);
            var email = new MimeMessage();            
            email.From.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", "francisopogah@gmail.com"));
            //email.From.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", "gmt.dailycheck@gmt-limited.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));            
            email.Subject = dailyCheck.Location == "Apapa" ? "DAILY CHECK" : dailyCheck.Location == "Abule-Oshun"
                ? "OFFDOCK AND BMS DAILY CHECK" : "DAILY CHECK";
            var bodyBuilder = new BodyBuilder();            

            bodyBuilder.HtmlBody = viewHtml.Replace("[url]", url);

            email.Body = bodyBuilder.ToMessageBody();
            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(host, port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mail, password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        private async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            string assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var metadataReference = Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(Path.Combine(assemblyFolder, "ITDailycheck.dll"));
            var engine = new RazorLightEngineBuilder()
             .UseMemoryCachingProvider()
             .UseEmbeddedResourcesProject(typeof(EmailTemplateModel))
             .EnableDebugMode(true)
             .AddMetadataReferences(metadataReference)
             .Build();
            string viewHtml = await engine.CompileRenderAsync<object>(viewName, model);

            return viewHtml;
        }
    }    

}
