using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class ScheduledNotificationConfiguration : IEntityTypeConfiguration<ScheduledNotification>
    {
        public void Configure(EntityTypeBuilder<ScheduledNotification> builder)
        {
            builder.HasKey(sn => sn.Id);
            
            builder.Property(sn => sn.Type)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(sn => sn.Title)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(sn => sn.Message)
                   .HasMaxLength(1000);
                   
            builder.Property(sn => sn.ScheduledFor)
                   .IsRequired();
                   
            builder.Property(sn => sn.Status)
                   .HasDefaultValue("pending")
                   .HasMaxLength(50);
                   
            builder.Property(sn => sn.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(sn => sn.User)
                   .WithMany()
                   .HasForeignKey(sn => sn.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}