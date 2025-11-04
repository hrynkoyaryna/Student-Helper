namespace DAL.Models
{
    public class SecurityLog
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Metadata { get; set; } // JSON
        
        // Foreign keys
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}