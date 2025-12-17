using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class NotificationSettingConfiguration : IEntityTypeConfiguration<NotificationSetting>
    {
        public void Configure(EntityTypeBuilder<NotificationSetting> builder)
        {
            builder.HasKey(ns => ns.UserId);
            
            builder.Property(ns => ns.PushEnabled)
                   .HasDefaultValue(true);
                   
            builder.Property(ns => ns.EmailEnabled)
                   .HasDefaultValue(true);
                   
            builder.Property(ns => ns.TelegramConnected)
                   .HasDefaultValue(false);
                   
            builder.Property(ns => ns.TelegramChatId)
                   .HasMaxLength(100);
                   
            builder.Property(ns => ns.Timezone)
                   .HasDefaultValue("UTC")
                   .HasMaxLength(50);
                   
            //builder.Property(ns => ns.NotificationTime)
                  // .HasDefaultValue("09:00");

            // Відносини
            builder.HasOne(ns => ns.User)
                   .WithOne(u => u.NotificationSetting)
                   .HasForeignKey<NotificationSetting>(ns => ns.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}