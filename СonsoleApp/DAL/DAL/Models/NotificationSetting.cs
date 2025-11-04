namespace DAL.Models
{
    public class NotificationSetting
    {
        public int UserId { get; set; }
        public bool PushEnabled { get; set; } = true;
        public int[] RemindBeforeMinutes { get; set; } = new int[] { 15, 30 };
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ScheduledFor { get; set; } 
        
        public User? User { get; set; }
    }
}