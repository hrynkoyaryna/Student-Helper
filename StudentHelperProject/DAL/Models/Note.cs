namespace DAL.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPinned { get; set; } = false;
        public string Content { get; set; } = string.Empty;

        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
        
        public ICollection<NoteLink> NoteLinks { get; set; } = new List<NoteLink>();
    }
}