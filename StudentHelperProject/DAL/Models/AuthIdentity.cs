namespace DAL.Models
{
    public class AuthIdentity
    {

    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty; 
    public string ExternalSubjectId { get; set; } = string.Empty;
    public string ExternalEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
    

        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}