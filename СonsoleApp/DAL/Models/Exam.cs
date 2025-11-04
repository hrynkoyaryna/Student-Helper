namespace DAL.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ExamDate { get; set; }
        public TimeSpan? StartAt { get; set; }
        public TimeSpan? EndAt { get; set; }
        public string Description { get; set; }
        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        
        public ICollection<NoteLink> NoteLinks { get; set; } = new List<NoteLink>();
    }
}