namespace DAL.Models
{
    public class ScheduleSource
    {
        public int Id { get; set; }
        public string SourceType { get; set; } // 'university_api', 'file', 'link', 'manual'
        public string SourceUrl { get; set; }
        public string FileRef { get; set; }
        public DateTime? LastSyncAt { get; set; }
        public string LastSyncStatus { get; set; }
        public string Name { get; set; } = string.Empty;

        
        // Foreign keys
        public int? UserId { get; set; }
        public User User { get; set; }
        
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}