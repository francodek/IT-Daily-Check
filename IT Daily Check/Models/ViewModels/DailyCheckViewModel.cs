using System.ComponentModel.DataAnnotations;

namespace IT_Daily_Check.Models.ViewModels
{
    public class DailyCheckViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_Created { get; set; }
        [Required]
        public string Location { get; set; }
        public string Created_By { get; set; }
        public List<InternetServiceSpeedcheckViewModel> InternetServiceSpeedcheckViewModels { get; set; }
        public List<DeviceServicecheckViewModel> DeviceServicecheckViewModels { get; set; }
        public List<CCTVcheckViewModel> CCTVcheckViewModels { get; set; }
    }
}
