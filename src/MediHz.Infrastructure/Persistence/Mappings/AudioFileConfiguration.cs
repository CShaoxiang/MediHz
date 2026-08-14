using MediHz.Domain.Audio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class AudioFileConfiguration : IEntityTypeConfiguration<AudioFile>
{
    public void Configure(EntityTypeBuilder<AudioFile> builder)
    {
        builder.ToTable("AudioFiles", table =>
        {
            table.HasCheckConstraint(
                "CK_AudioFiles_Format",
                "\"Format\" IN (0, 1)");
            
            table.HasCheckConstraint(
                "CK_AudioFiles_SamplingRate_Positive",
                "\"SampleRate\" > 0");

            table.HasCheckConstraint(
                "CK_AudioFiles_Channels_Positive",
                "\"Channels\" > 0");
        });

        builder.HasKey(audioFile => audioFile.Id);

        builder.Property(audioFile => audioFile.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(audioFile => audioFile.Format)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(audioFile => audioFile.Duration)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(audioFile => audioFile.SampleRate)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(audioFile => audioFile.Channels)
            .IsRequired();
        // Add these back once Projects and Users have been implemented
        // builder.Property(audioFile => audioFile.ProjectId).IsRequired();
        // builder.Property(audioFile => audioFile.UploaderId).IsRequired();

        builder.HasIndex(audioFile => audioFile.ProjectId);
        builder.HasIndex(audioFile => audioFile.UploaderId);
        
        builder.Property(audioFile => audioFile.SizeBytes)
            .IsRequired();
        
        // Relationships
        
        // Cascade : Remove associated Audio Files
        builder.HasOne(audioFile => audioFile.Project)
            .WithMany(project => project.AudioFiles)
            .HasForeignKey(audioFile => audioFile.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Restrict : Prevent User from being deleted if active audio files exist
        builder.HasOne(audioFile => audioFile.User)
            .WithMany()
            .HasForeignKey(audioFile => audioFile.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
