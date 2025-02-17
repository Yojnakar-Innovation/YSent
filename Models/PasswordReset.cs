using System.ComponentModel.DataAnnotations;

namespace YSent.Models
{
    public class PasswordReset
    {
        
        [EmailAddress]
        public string Email { get; set; }

        public string ResetToken { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
