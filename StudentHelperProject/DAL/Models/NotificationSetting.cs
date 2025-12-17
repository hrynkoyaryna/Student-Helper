namespace DAL.Models
{
    public class NotificationSetting
    {
        public int UserId { get; set; }
        public bool PushEnabled { get; set; } = true;
        public int[] RemindBeforeMinutes { get; set; } = new int[] { 15, 30 };
        public bool TelegramConnected { get; set; } = false;
        public string Timezone { get; set; } = "UTC";
        public bool EmailEnabled { get; set; }
        public string TelegramChatId { get; set; } = string.Empty;

        
        public User User { get; set; }
    }
}