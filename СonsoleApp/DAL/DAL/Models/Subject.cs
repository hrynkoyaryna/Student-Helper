namespace DAL.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string DefaultColor { get; set; }
        
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}