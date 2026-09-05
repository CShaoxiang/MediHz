using MediHz.Domain.Users;

namespace MediHz.Domain.Projects;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    // join many to many
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
    public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Owner;
    public DateTimeOffset JoinedAt { get; set; } = DateTime.UtcNow;
}
