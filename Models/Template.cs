using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YSent.Models
{
    public class Template
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment the ID
        public int Id { get; set; }

        [Required] 
        [StringLength(255)] 
        public string TemplateName { get; set; } = string.Empty;

      
        public string PlainText { get; set; } = string.Empty;

        
        public string HtmlContent { get; set; } = string.Empty;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ImageUrl { get; set; }


        public int? UserId { get; set; } // Nullable in case templates can exist without a user
    }
}