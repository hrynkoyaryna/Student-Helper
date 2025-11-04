namespace DAL.Models
{
    public class ScheduledNotification
    {
        public int Id { get; set; }
        public string Channel { get; set; } // 'push'
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public DateTime FireAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string Status { get; set; } = "pending"; // 'pending', 'sent', 'failed'
        public string ErrorMessage { get; set; }
        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
    }
}