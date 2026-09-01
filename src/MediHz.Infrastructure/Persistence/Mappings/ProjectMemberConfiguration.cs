using MediHz.Domain.Projects;
using MediHz.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(member => new { member.ProjectId, member.UserId });

        builder.Property(member => member.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(member => member.JoinedAt)
            .IsRequired();
        
        // Relationships
        
        // Cascade : Remove all members if project delete 
        // TODO: Add this back oncce Project<->User mapping is restored.
        builder.HasOne<Project>()
            .WithMany(project => project.Members)
            .HasForeignKey(member => member.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Cascade :  Delete all 
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(member => member.UserId);
    }
}
