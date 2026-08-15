using MediHz.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(project => project.OwnerId);
        
        // Relationships
        
        // Restrict : Prevent owner from being deleted if own project
        // TODO: Add this back oncce Project<->User mapping is restored.
        // builder.HasOne(project => project.Owner)
        //     .WithMany(user => user.OwnedProjects)
        //     .HasForeignKey(project => project.OwnerId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
