using System.ComponentModel.DataAnnotations.Schema;

namespace IT_Daily_Check.Models
{
    public class InternetServiceSpeedcheck
    {

        public int Id { get; set; }

        public string ISP_NAME { get; set; }
        public decimal DownloadSpeed { get; set; }
        public decimal UploadSpeed { get; set; }

        [ForeignKey("DailyChecks")]
        public int DailyChecksId { get; set; }
        public virtual DailyCheck DailyChecks { get; set; }
    }
}
