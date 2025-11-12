namespace DAL.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int Attempts { get; set; } = 0;
        public bool IsUsed { get; set; }
public DateTime CreatedAt { get; set; }

        
        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }
    }
}