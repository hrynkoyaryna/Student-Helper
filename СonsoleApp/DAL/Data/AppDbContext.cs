using Microsoft.EntityFrameworkCore;
using DAL.Models;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
            
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasIndex(t => t.DueDate).IsUnique(false); 

                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tasks)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<NotificationSetting>()
                .HasKey(ns => ns.UserId);
            modelBuilder.Entity<NotificationSetting>()
                .HasOne(ns => ns.User)
                .WithOne(u => u.NotificationSetting)
                .HasForeignKey<NotificationSetting>(ns => ns.UserId);
                
            modelBuilder.Entity<Profile>()
                .HasKey(p => p.UserId);
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.UserId);

            modelBuilder.Entity<AuthIdentity>()
                .HasIndex(a => new { a.Provider, a.ExternalSubjectId })
                .IsUnique();
            
            modelBuilder.Entity<NoteLink>()
                .HasIndex(nl => new { nl.LinkType, nl.LinkId, nl.NoteId })
                .IsUnique(false);
            
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<PasswordResetToken>().HasIndex(t => t.Token).IsUnique();
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
                if (entityEntry.State == EntityState.Added)
                {
                    var createdAtProperty = entityEntry.Metadata.FindProperty("CreatedAt");
                    if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
                    {
                        entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }

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