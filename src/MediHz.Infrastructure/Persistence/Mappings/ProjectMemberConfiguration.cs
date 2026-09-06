using MediHz.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediHz.Infrastructure.Persistence.Mappings;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(member => new { member.ProjectId, member.UserId });

        builder.Property(member => member.Role).HasConversion<int>().IsRequired();

        builder.Property(member => member.JoinedAt).IsRequired();

        // Relationships

        // Cascade : Remove all members if project delete
        builder
            .HasOne(member => member.Project)
            .WithMany(project => project.Members)
            .HasForeignKey(member => member.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade :  Delete all
        builder
            .HasOne(member => member.User)
            .WithMany(user => user.Members)
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(member => member.UserId);
    }
}
