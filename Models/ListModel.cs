using System.ComponentModel.DataAnnotations;

namespace YSent.Models
{
    public class ListModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "List Name is required")]
        [Display(Name = "List Name")]
        public string ListName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
