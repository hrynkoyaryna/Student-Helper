using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(e => e.Type)
                   .HasDefaultValue("personal")
                   .HasMaxLength(50);
                   
            builder.Property(e => e.Description)
                   .HasMaxLength(1000);
                   
            builder.Property(e => e.IsAllDay)
                   .HasDefaultValue(false);
                   
            builder.Property(e => e.StartAt)
                   .IsRequired();
                   
            builder.Property(e => e.EndAt)
                   .IsRequired();
                   
            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("NOW()");
                   
            builder.Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Events)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Subject)
                   .WithMany(s => s.Events)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Lecturer)
                   .WithMany(l => l.Events)
                   .HasForeignKey(e => e.LecturerId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Room)
                   .WithMany(r => r.Events)
                   .HasForeignKey(e => e.RoomId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.ScheduleSource)
                   .WithMany(s => s.Events)
                   .HasForeignKey(e => e.SourceId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Індекси
            builder.HasIndex(e => new { e.UserId, e.StartAt });
            builder.HasIndex(e => e.StartAt);
        }
    }
}