using IT_Daily_Check.Models;
using System.Threading.Tasks;

namespace ITDailyCheck.Services.Interfaces
{
    public interface IDailyCheckEmailService
    {
        Task SendDailyCheckToEmail(DailyCheck dailyCheck, string toEmail, string url, string host, int port, string mail, string password);
    }
}
