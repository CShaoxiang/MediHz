using MediHz.Domain.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class AnnotationConfiguration : IEntityTypeConfiguration<Annotation>
{
    public void Configure(EntityTypeBuilder<Annotation> builder)
    {
        builder.ToTable("Annotations", table =>
        {
            table.HasCheckConstraint(
                "CK_Annotations_TimeRange",
                "\"StartTimeSeconds\" >= 0 AND \"EndTimeSeconds\" > \"StartTimeSeconds\" AND \"DurationSeconds\" >= 0.1");
        });

        builder.HasKey(annotation => annotation.Id);

        builder.Property(annotation => annotation.StartTimeSeconds)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(annotation => annotation.EndTimeSeconds)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(annotation => annotation.DurationSeconds)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(annotation => annotation.CreatedAtUtc)
            .IsRequired();

        builder.Property(annotation => annotation.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(annotation => annotation.AudioFileId);
        builder.HasIndex(annotation => annotation.UserId);

        // Relationships
        
        // Cascade : Remove associated annotations if audio file deleted
        builder.HasOne(annotation => annotation.AudioFile)
            .WithMany()
            .HasForeignKey(annotation => annotation.AudioFileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict : Prevent User from being deleted if active annotations exist
        builder.HasOne(annotation => annotation.User)
            .WithMany()
            .HasForeignKey(annotation => annotation.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
