namespace MediHz.Domain.Projects;

public class ProjectMember
{
    public Guid ProjectId { get; set; } 
    public Guid UserId { get; set; } 
    public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Owner;
    public DateTimeOffset JoinedAt { get; set; } = DateTime.UtcNow;
}