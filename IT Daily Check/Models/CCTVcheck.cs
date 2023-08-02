using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT_Daily_Check.Models
{
    public class CCTVcheck
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [Required]
        public string Results { get; set; }
        public string Reasons { get; set; }

        [Required]
        public string Comments { get; set; }
        public string Location { get; set; }

        [ForeignKey("DailyChecks")]
        public int DailyChecksId { get; set; }
        public DailyCheck DailyChecks { get; set; }

    }
}
