using System.ComponentModel.DataAnnotations;

namespace IT_Daily_Check.Models.ViewModels
{
    public class CCTVcheckViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public string Location { get; set; }
        [Required]
        public string Results { get; set; }
        [Required]
        public string Reasons { get; set; }
        [Required]
        public string Comments { get; set; }
    }
}
