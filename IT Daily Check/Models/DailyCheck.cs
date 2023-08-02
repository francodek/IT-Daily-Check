using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using IT_Daily_Check.Data;

namespace IT_Daily_Check.Models
{
    public class DailyCheck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_Created { get; set; }
        public string Location { get; set; }
        public string Created_By { get; set; }
        public string ImageOneName { get; set; }
        public string ImageTwoName { get; set; }
        public string Comments { get; set; }

        [NotMapped]
        [FileExtension]
        [Display(Name = "Choose Image")]
        public IFormFile ImageUploadOne { get; set; }

        [NotMapped]
        [FileExtension]
        [Display(Name = "Choose Image")]
        public IFormFile ImageUploadTwo { get; set; }

        public ICollection<CCTVcheck> CCTVchecks { get; set; }
        public ICollection<DeviceServicecheck> DeviceServicechecks { get; set; }

        public ICollection<InternetServiceSpeedcheck> InternetServiceSpeedchecks { get; set; }
    }
}

