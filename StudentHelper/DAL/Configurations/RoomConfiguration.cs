using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(r => r.Building)
                   .HasMaxLength(100);
                   
            builder.Property(r => r.Capacity);

            // Відносини
            builder.HasMany(r => r.Events)
                   .WithOne(e => e.Room)
                   .HasForeignKey(e => e.RoomId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}