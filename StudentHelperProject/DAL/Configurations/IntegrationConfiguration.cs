using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class IntegrationConfiguration : IEntityTypeConfiguration<Integration>
    {
        public void Configure(EntityTypeBuilder<Integration> builder)
        {
            builder.HasKey(i => i.Id);
            
            builder.Property(i => i.Provider)
                   .IsRequired()
                   .HasMaxLength(50);
                   
            //builder.Property(i => i.AccessToken)
                 //  .HasMaxLength(1000);
                   
           // builder.Property(i => i.RefreshToken)
               //    .HasMaxLength(1000);
                   
           // builder.Property(i => i.ExpiresAt);
                   
            builder.Property(i => i.CreatedAt)
                   .HasDefaultValueSql("NOW()");
                   
            builder.Property(i => i.UpdatedAt)
                   .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(i => i.User)
                   .WithMany()
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Індекси
            builder.HasIndex(i => new { i.UserId, i.Provider }).IsUnique();
        }
    }
}