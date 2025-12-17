namespace DAL.Models
{
    public class Profile
    {
        public int UserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Locale { get; set; } = "en";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; }

        public string Timezone { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

    }
}