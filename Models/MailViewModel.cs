using System;

namespace YSent.Models
{
    public class MailViewModel
    {
        public int Id { get; set; }  // Unique identifier 
        public string Subject { get; set; }
        public string RecipientEmails { get; set; }
        public bool IsSent { get; set; }  // True if the email is sent; false if it's a draft
        public DateTime? SentTime { get; set; }
    }
}
 