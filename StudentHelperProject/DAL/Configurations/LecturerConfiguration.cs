using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class LecturerConfiguration : IEntityTypeConfiguration<Lecturer>
    {
        public void Configure(EntityTypeBuilder<Lecturer> builder)
        {
            builder.HasKey(l => l.Id);
            
            builder.Property(l => l.FullName)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(l => l.Email)
                   .HasMaxLength(255);
                   
            builder.Property(l => l.Phone)
                   .HasMaxLength(20);
                   
            //builder.Property(l => l.Department)
                  // .HasMaxLength(100);

            // Відносини
            builder.HasMany(l => l.Events)
                   .WithOne(e => e.Lecturer)
                   .HasForeignKey(e => e.LecturerId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}