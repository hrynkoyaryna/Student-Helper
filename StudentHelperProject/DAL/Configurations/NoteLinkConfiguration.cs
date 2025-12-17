using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
    public class NoteLinkConfiguration : IEntityTypeConfiguration<NoteLink>
    {
        public void Configure(EntityTypeBuilder<NoteLink> builder)
        {
            builder.HasKey(nl => nl.Id);
            
            //builder.Property(nl => nl.LinkedEntityType)
                 //  .IsRequired()
                  // .HasMaxLength(50);
                   
           // builder.Property(nl => nl.LinkedEntityId)
                //   .IsRequired();
                   
            //builder.Property(nl => nl.CreatedAt)
                  // .HasDefaultValueSql("NOW()");

            // Відносини
            builder.HasOne(nl => nl.Note)
                   .WithMany(n => n.NoteLinks)
                   .HasForeignKey(nl => nl.NoteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}