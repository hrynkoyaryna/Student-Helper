using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.HasKey(p => p.UserId);
            
            builder.Property(p => p.Locale)
                   .HasDefaultValue("en")
                   .HasMaxLength(10);
                   
            builder.Property(p => p.Timezone)
                   .HasMaxLength(50);
                   
            builder.Property(p => p.AvatarUrl)
                   .HasMaxLength(500);
                   
            builder.Property(p => p.Phone)
                   .HasMaxLength(20);
                   
            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Profile)
                   .HasForeignKey<Profile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}