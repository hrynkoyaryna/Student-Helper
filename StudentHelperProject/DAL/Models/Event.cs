namespace DAL.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Type { get; set; } // 'personal', 'academic', 'online'
        public string Description { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceExceptions { get; set; }
        public bool IsAllDay { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        
        public int? LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }
        
        public int? RoomId { get; set; }
        public Room Room { get; set; }
        
        public int? SourceId { get; set; }
        public ScheduleSource ScheduleSource { get; set; }
        
        public ICollection<NoteLink> NoteLinks { get; set; } = new List<NoteLink>();
    }
}