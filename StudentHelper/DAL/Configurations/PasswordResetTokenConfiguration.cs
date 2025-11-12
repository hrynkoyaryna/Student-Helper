using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Token)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(p => p.ExpiresAt)
                   .IsRequired();
                   
            builder.Property(p => p.IsUsed)
                   .HasDefaultValue(false);
                   
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Унікальний індекс для токена
            builder.HasIndex(p => p.Token).IsUnique();

            // Відносини
            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}