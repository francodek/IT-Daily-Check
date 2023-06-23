using System.ComponentModel.DataAnnotations;

namespace IT_Daily_Check.Models.ViewModels
{
    public class InternetServiceSpeedcheckViewModel
    {
        public int Id { get; set; }
        public string ISP_NAME { get; set; }
        [Required]
        public decimal DownloadSpeed { get; set; }
        [Required]
        public decimal UploadSpeed { get; set; }
    }
}
