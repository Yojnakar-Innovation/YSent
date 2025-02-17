namespace YSent.Models
{
    public class PasswordResetToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
    }
}
