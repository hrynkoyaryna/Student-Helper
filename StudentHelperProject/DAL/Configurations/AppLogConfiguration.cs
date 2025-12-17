using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class AppLogConfiguration : IEntityTypeConfiguration<AppLog>
    {
        public void Configure(EntityTypeBuilder<AppLog> builder)
        {
            builder.HasKey(al => al.Id);
            
            builder.Property(al => al.Level)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(al => al.Message)
                   .IsRequired()
                   .HasColumnType("text");
                   
            builder.Property(al => al.Exception)
                   .HasColumnType("text");
                   
            builder.Property(al => al.Logger)
                   .HasMaxLength(255);
                   
            builder.Property(al => al.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Індекси
            builder.HasIndex(al => al.CreatedAt);
            builder.HasIndex(al => al.Level);
        }
    }
}