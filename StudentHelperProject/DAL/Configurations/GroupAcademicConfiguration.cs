using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class GroupAcademicConfiguration : IEntityTypeConfiguration<GroupAcademic>
    {
        public void Configure(EntityTypeBuilder<GroupAcademic> builder)
        {
            builder.HasKey(g => g.Id);
            
            builder.Property(g => g.Code)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(g => g.Faculty)
                   .IsRequired()
                   .HasMaxLength(100);
                   
            builder.Property(g => g.Degree)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(g => g.Year)
                   .IsRequired();

            // Відносини
            builder.HasMany(g => g.Users)
                   .WithOne(u => u.Group)
                   .HasForeignKey(u => u.GroupId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Індекси
            builder.HasIndex(g => g.Code).IsUnique();
        }
    }
}