namespace DAL.Models
{
    public class Integration
    {
        public int Id { get; set; }
        public string Provider { get; set; } // 'google_calendar'
        public string OAuthAccessToken { get; set; }
        public string OAuthRefreshToken { get; set; }
        public DateTime? OAuthExpiresAt { get; set; }
        public string Metadata { get; set; } // JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
    }
}