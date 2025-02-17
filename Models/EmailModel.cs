using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YSent.Models
{
    public class EmailModel
    {
        public int Id { get; set; } // Used to track draft IDs

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Recipient emails are required")]
        public string RecipientEmails { get; set; }

        [Required(ErrorMessage = "Email body is required")]
        public string Body { get; set; }

        public DateTime? SentTime { get; set; }
        public DateTime? OpenDate { get; set; }
        public string opened { get; set; }
        public string TrackingId { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }

        // This flag determines if the email is saved as a draft.
        public bool IsDraft { get; set; } = false;

        // Optional template ID for pre-built email content.
        public int? TemplateId { get; set; }

        // These lists are used to populate drop-down menus in the view.
        public List<Template> AvailableTemplates { get; set; }
        public string SelectedListName { get; set; }
        public List<string> AvailableMailingLists { get; set; }
    }
}
