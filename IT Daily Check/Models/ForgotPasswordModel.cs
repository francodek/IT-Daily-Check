using System.ComponentModel.DataAnnotations;

namespace IT_Daily_Check.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
