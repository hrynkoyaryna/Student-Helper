namespace DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsNotified { get; set; } = true;
        public int DaysForNotification { get; set; } = 1;

        // Password reset fields
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpires { get; set; }

        public int? GroupId { get; set; }
        public GroupAcademic Group { get; set; }

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<AuthIdentity> AuthIdentities { get; set; } = new List<AuthIdentity>();
        public Profile Profile { get; set; }
        public NotificationSetting NotificationSetting { get; set; }
    }
}