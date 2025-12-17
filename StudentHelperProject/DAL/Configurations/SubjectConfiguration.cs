using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(s => s.ShortName)
                   .HasMaxLength(50);
                   
            builder.Property(s => s.DefaultColor)
                   .HasMaxLength(7); // Формат HEX кольору
            
            builder.Property(s => s.Description)
                   .HasMaxLength(1000);

            // Відносини
            builder.HasMany(s => s.Events)
                   .WithOne(e => e.Subject)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.SetNull);
                   
            builder.HasMany(s => s.Tasks)
                   .WithOne(t => t.Subject)
                   .HasForeignKey(t => t.SubjectId)
                   .OnDelete(DeleteBehavior.SetNull);
                   
            builder.HasMany(s => s.Exams)
                   .WithOne(e => e.Subject)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}