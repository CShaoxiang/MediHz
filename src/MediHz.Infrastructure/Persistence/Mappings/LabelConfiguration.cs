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

        builder.HasData(
            new Label
            {
                Id = PrePackagedLabels.CirCorS1Id,
                Name = "cardiac.s1",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorSystoleId,
                Name = "cardiac.systole",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorS2Id,
                Name = "cardiac.s2",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorDiastoleId,
                Name = "cardiac.diastole",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorUnknownId,
                Name = "cardiac.unknown",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorSystolicMurmurId,
                Name = "cardiac.murmur.systolic",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.CirCorDiastolicMurmurId,
                Name = "cardiac.murmur.diastolic",
                LabelGroupId = PrePackagedLabels.CirCorGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungInhalationId,
                Name = "resp.phase.inhalation",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungExhalationId,
                Name = "resp.phase.exhalation",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungWheezeId,
                Name = "resp.cas.wheeze",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungStridorId,
                Name = "resp.cas.stridor",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungRhonchiId,
                Name = "resp.cas.rhonchi",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            },
            new Label
            {
                Id = PrePackagedLabels.HfLungDasId,
                Name = "resp.das.crackle",
                LabelGroupId = PrePackagedLabels.HfLungGroupId,
                OwnerId = null,
                CreatedAtUtc = PrePackagedLabels.SeededAtUtc,
            }
        );
    }
}
