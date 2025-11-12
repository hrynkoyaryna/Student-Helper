using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.HasIndex(u => u.Email).IsUnique();
            
            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(u => u.FirstName)
                   .HasMaxLength(100);
                   
            builder.Property(u => u.LastName)
                   .HasMaxLength(100);
                   
            builder.Property(u => u.Status)
                   .HasDefaultValue("active")
                   .HasMaxLength(50);
                   
            builder.Property(u => u.CreatedAt)
                   .HasDefaultValueSql("NOW()");
                   
            builder.Property(u => u.UpdatedAt)
                   .HasDefaultValueSql("NOW()");
                   
            builder.Property(u => u.IsNotified)
                   .HasDefaultValue(true);
                   
            builder.Property(u => u.DaysForNotification)
                   .HasDefaultValue(1);

            // Відносини
            builder.HasOne(u => u.Group)
                   .WithMany(g => g.Users)
                   .HasForeignKey(u => u.GroupId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.AuthIdentities)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId);
                   
            builder.HasMany(u => u.Events)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId);
                   
            builder.HasMany(u => u.Tasks)
                   .WithOne(t => t.User)
                   .HasForeignKey(t => t.UserId);
                   
            builder.HasMany(u => u.Exams)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId);
                   
            builder.HasMany(u => u.Notes)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId);

            builder.HasOne(u => u.Profile)
                   .WithOne(p => p.User)
                   .HasForeignKey<Profile>(p => p.UserId);
                   
            builder.HasOne(u => u.NotificationSetting)
                   .WithOne(ns => ns.User)
                   .HasForeignKey<NotificationSetting>(ns => ns.UserId);
        }
    }
}