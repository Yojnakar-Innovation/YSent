namespace YSent.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ListName { get; set; }
        public DateTime SubscribedDate { get; set; }
        public string Status { get; set; } // "Active", "Unsubscribed", "Bounced"
    }
}
