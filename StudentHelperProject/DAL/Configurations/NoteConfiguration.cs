using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Models;

namespace DAL.Configurations
{
       public class NoteConfiguration : IEntityTypeConfiguration<Note>
       {
              public void Configure(EntityTypeBuilder<Note> builder)
              {
                     builder.HasKey(n => n.Id);

                     builder.Property(n => n.Title)
                            .IsRequired()
                            .HasMaxLength(200);

                     builder.Property(n => n.Body)
                            .HasColumnType("text")
                            .IsRequired(false);

                     builder.Property(n => n.Content)
                            .HasColumnType("text")
                            .IsRequired(false);

                     builder.Property(n => n.IsPinned)
                            .HasDefaultValue(false);

                     builder.Property(n => n.CreatedAt)
                            .HasDefaultValueSql("NOW()");

                     builder.Property(n => n.UpdatedAt)
                            .HasDefaultValueSql("NOW()");

                     // Відносини
                     builder.HasOne(n => n.User)
                            .WithMany(u => u.Notes)
                            .HasForeignKey(n => n.UserId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(n => n.NoteLinks)
                            .WithOne(nl => nl.Note)
                            .HasForeignKey(nl => nl.NoteId)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}