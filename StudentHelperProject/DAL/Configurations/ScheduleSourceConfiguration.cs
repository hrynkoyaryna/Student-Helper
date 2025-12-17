using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class ScheduleSourceConfiguration : IEntityTypeConfiguration<ScheduleSource>
    {
        public void Configure(EntityTypeBuilder<ScheduleSource> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            //builder.Property(s => s.Url)
               //    .HasMaxLength(500);
                   
           // builder.Property(s => s.Type)
                //   .IsRequired()
                //   .HasMaxLength(50);
                   
           // builder.Property(s => s.LastSync)
                 //  .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(s => s.User)
                   .WithMany()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
                   
            builder.HasMany(s => s.Events)
                   .WithOne(e => e.ScheduleSource)
                   .HasForeignKey(e => e.SourceId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}