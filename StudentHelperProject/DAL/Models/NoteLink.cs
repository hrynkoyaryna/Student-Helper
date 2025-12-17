namespace DAL.Models
{
    public class NoteLink
    {
        public int Id { get; set; }
        public string LinkType { get; set; } // 'timetable_event', 'task', 'exam'
        public int LinkId { get; set; }
        
        // Foreign keys
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}