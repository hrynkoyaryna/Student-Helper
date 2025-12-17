using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(e => e.Description)
                   .HasMaxLength(1000);
                   
            builder.Property(e => e.ExamDate)
                   .IsRequired();
                   
            builder.Property(e => e.StartAt);
                   
            builder.Property(e => e.EndAt);

            // Відносини
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Exams)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Subject)
                   .WithMany(s => s.Exams)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Індекси
            builder.HasIndex(e => new { e.UserId, e.ExamDate });
            builder.HasIndex(e => e.ExamDate);
        }
    }
}