using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagementAPI.Domain.Entities.Documents;

namespace PropertyManagementAPI.Infrastructure.Data.Configurations.Documents
{
    public class DocumentReferenceConfiguration : IEntityTypeConfiguration<DocumentReference>
    {
        public void Configure(EntityTypeBuilder<DocumentReference> builder)
        {
            builder.ToTable("DocumentReferences");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.RelatedEntityType).IsRequired().HasMaxLength(50);
            builder.Property(r => r.RelatedEntityId).IsRequired();
            builder.Property(r => r.AccessRole).IsRequired().HasMaxLength(50).HasDefaultValue("Viewer");
            builder.Property(r => r.Description).HasMaxLength(500);
            builder.HasIndex(r => new { r.RelatedEntityType, r.RelatedEntityId });
            builder.HasIndex(r => new { r.DocumentId, r.AccessRole });
        }
    }
}