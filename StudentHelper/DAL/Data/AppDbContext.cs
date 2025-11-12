using Microsoft.EntityFrameworkCore;
using DAL.Models;
using System.Reflection;

namespace DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets для всіх таблиць
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<GroupAcademic> GroupAcademics { get; set; } = null!;
        public DbSet<AuthIdentity> AuthIdentities { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Lecturer> Lecturers { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<ScheduleSource> ScheduleSources { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<DAL.Models.Task> Tasks { get; set; } = null!;
        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<NoteLink> NoteLinks { get; set; } = null!;
        public DbSet<NotificationSetting> NotificationSettings { get; set; } = null!;
        public DbSet<Integration> Integrations { get; set; } = null!;
        public DbSet<ScheduledNotification> ScheduledNotifications { get; set; } = null!;
        public DbSet<SecurityLog> SecurityLogs { get; set; } = null!;
        public DbSet<AppLog> AppLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            CreateAdditionalIndexes(modelBuilder);
        }

        private void CreateAdditionalIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email);
            modelBuilder.Entity<Event>().HasIndex(e => new { e.UserId, e.StartAt });
            modelBuilder.Entity<DAL.Models.Task>().HasIndex(t => new { t.UserId, t.DueAt });
            modelBuilder.Entity<Exam>().HasIndex(e => new { e.UserId, e.ExamDate });
            modelBuilder.Entity<AuthIdentity>().HasIndex(a => new { a.UserId, a.Provider });
            modelBuilder.Entity<SecurityLog>().HasIndex(sl => sl.CreatedAt);
            modelBuilder.Entity<AppLog>().HasIndex(al => al.CreatedAt);
            modelBuilder.Entity<AppLog>().HasIndex(al => al.Level);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=student_helper;Username=postgres;Password=Kvitochka06");
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
{
    var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

    foreach (var entityEntry in entries)
    {
        // Оновлення CreatedAt для нових записів
        if (entityEntry.State == EntityState.Added)
        {
            var createdAtProperty = entityEntry.Metadata.FindProperty("CreatedAt");
            if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
            {
                entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }
        }

        // Оновлення UpdatedAt для змінених записів
        var updatedAtProperty = entityEntry.Metadata.FindProperty("UpdatedAt");
        if (updatedAtProperty != null && 
            (updatedAtProperty.ClrType == typeof(DateTime) || updatedAtProperty.ClrType == typeof(DateTime?)))
        {
            entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }
    }
}
    }
}