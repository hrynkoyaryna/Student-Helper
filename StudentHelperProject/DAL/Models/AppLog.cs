namespace DAL.Models
{
    public class AppLog
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Context { get; set; } // JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Exception { get; set; } = string.Empty;
        public string Logger { get; set; } = string.Empty;

    }
}