namespace DAL.Models
{
    public class AuthIdentity
    {
        public int Id { get; set; }
        public string Provider { get; set; } 
        public string ExternalSubjectId { get; set; }
        public string ExternalEmail { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
    }
}