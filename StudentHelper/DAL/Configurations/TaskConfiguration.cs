using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<DAL.Models.Task>
    {
        public void Configure(EntityTypeBuilder<DAL.Models.Task> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(t => t.Description)
                   .HasMaxLength(1000);
                   
            builder.Property(t => t.Status)
                   .HasDefaultValue("current")
                   .HasMaxLength(50);
                   
            builder.Property(t => t.Priority)
                   .HasDefaultValue("medium")
                   .HasMaxLength(50);
                   
            //builder.Property(t => t.DueAt);
                   
            //builder.Property(t => t.CompletedAt);
                   
            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("NOW()");
                   
            builder.Property(t => t.UpdatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(t => t.User)
                   .WithMany(u => u.Tasks)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Subject)
                   .WithMany(s => s.Tasks)
                   .HasForeignKey(t => t.SubjectId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Індекси
            builder.HasIndex(t => new { t.UserId, t.DueAt });
            builder.HasIndex(t => t.DueAt);
            builder.HasIndex(t => t.Status);
        }
    }
}