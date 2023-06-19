using System.ComponentModel.DataAnnotations.Schema;

namespace IT_Daily_Check.Models
{
    public class DeviceServicecheck
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Status { get; set; }

        [ForeignKey("DailyChecks")]
        public int DailyChecksId { get; set; }
        public virtual DailyCheck DailyChecks { get; set; }
    }
}
