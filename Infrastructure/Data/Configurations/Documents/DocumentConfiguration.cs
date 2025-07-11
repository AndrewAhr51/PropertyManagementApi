using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagementAPI.Domain.Entities.Documents;

namespace PropertyManagementAPI.Infrastructure.Data.Configurations.Documents
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(255);
            builder.Property(d => d.MimeType).IsRequired().HasMaxLength(100);
            builder.Property(d => d.SizeInBytes).IsRequired();
            builder.Property(d => d.DocumentType).HasMaxLength(100);
            builder.Property(d => d.CreateDate).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(d => d.CreatedByUserId).IsRequired();
            builder.Property(d => d.IsEncrypted).HasDefaultValue(false);
            builder.Property(d => d.Checksum).HasMaxLength(64);
            builder.Property(d => d.CorrelationId).HasMaxLength(128);
            builder.Property(d => d.Status).HasMaxLength(50).HasDefaultValue("Active");
            builder.Property(d => d.Content).IsRequired().HasColumnType("LONGBLOB");

            builder.HasMany(d => d.References)
                   .WithOne(r => r.Document)
                   .HasForeignKey(r => r.DocumentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}