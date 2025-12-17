using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class AuthIdentityConfiguration : IEntityTypeConfiguration<AuthIdentity>
    {
        public void Configure(EntityTypeBuilder<AuthIdentity> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.Property(a => a.Provider)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            builder.Property(a => a.ProviderUserId)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Унікальний індекс для пари UserId + Provider
            builder.HasIndex(a => new { a.UserId, a.Provider })
                   .IsUnique();

            // Відносини
            builder.HasOne(a => a.User)
                   .WithMany(u => u.AuthIdentities)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}