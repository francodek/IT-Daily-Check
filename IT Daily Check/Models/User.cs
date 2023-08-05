using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IT_Daily_Check.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Position { get; set; }
    }
}
