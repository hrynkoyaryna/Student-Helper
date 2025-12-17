namespace DAL.Models
{
    public class GroupAcademic
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Faculty { get; set; }
        public string Degree { get; set; }
        public int Year { get; set; }
        
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}