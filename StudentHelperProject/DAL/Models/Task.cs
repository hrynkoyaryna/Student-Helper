namespace DAL.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueAt { get; set; }
        public string Status { get; set; } = "pending"; // 'pending', 'completed', 'overdue'
        public string Priority { get; set; } = "medium"; // 'low', 'medium', 'high', 'urgent'
        public string Category { get; set; } = "Особисте"; // 'Особисте' або 'Навчання'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }

        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<NoteLink> NoteLinks { get; set; } = new List<NoteLink>();
    }
}