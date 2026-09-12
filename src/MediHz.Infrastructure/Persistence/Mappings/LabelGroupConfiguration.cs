using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class LabelGroupConfiguration : IEntityTypeConfiguration<LabelGroup>
{
    public void Configure(EntityTypeBuilder<LabelGroup> builder)
    {
        builder.ToTable("LabelGroup");

        builder.HasKey(labelGroup => labelGroup.Id);
        builder.Property(labelGroup => labelGroup.Name).IsRequired().HasMaxLength(150);
        builder.Property(labelGroup => labelGroup.CreatedAtUtc).IsRequired();

        // Relations
        builder
            .HasMany(labelGroup => labelGroup.Labels)
            .WithOne(label => label.LabelGroup)
            .HasForeignKey(label => label.LabelGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(labelGroup => labelGroup.Owner)
            .WithMany()
            .HasForeignKey(labelGroup => labelGroup.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
