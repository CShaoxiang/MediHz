using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Label");

        builder.HasKey(label => label.Id);
        builder.Property(label => label.Name).IsRequired().HasMaxLength(150);
        builder.Property(label => label.CreatedAtUtc).IsRequired();

        // Relations
        builder
            .HasOne(label => label.LabelGroup)
            .WithMany(labelGroup => labelGroup.Labels)
            .HasForeignKey(label => label.LabelGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(label => label.Owner)
            .WithMany()
            .HasForeignKey(label => label.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
