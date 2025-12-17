using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class SecurityLogConfiguration : IEntityTypeConfiguration<SecurityLog>
    {
        public void Configure(EntityTypeBuilder<SecurityLog> builder)
        {
            builder.HasKey(sl => sl.Id);
            
            //builder.Property(sl => sl.Action)
              //     .IsRequired()
                 //  .HasMaxLength(100);
                   
            builder.Property(sl => sl.IpAddress)
                   .HasMaxLength(45); // IPv6 support
                   
            builder.Property(sl => sl.UserAgent)
                   .HasMaxLength(500);
                   
           // builder.Property(sl => sl.Details)
                  // .HasColumnType("text");
                   
            builder.Property(sl => sl.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(sl => sl.User)
                   .WithMany()
                   .HasForeignKey(sl => sl.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Індекси
            builder.HasIndex(sl => sl.CreatedAt);
            builder.HasIndex(sl => new { sl.UserId, sl.CreatedAt });
        }
    }
}