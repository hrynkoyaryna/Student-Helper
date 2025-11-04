namespace DAL.Models
{
    public class Lecturer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}