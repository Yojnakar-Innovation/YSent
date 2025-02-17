using System.ComponentModel.DataAnnotations;

namespace YSent.Models
{
    public class UserEntry
    {
        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(30)]
        public string username { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string password { get; set; }


        public DateTime created_at { get; set; }
        public bool? is_admin { get; set; }
    }
}