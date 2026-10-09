namespace MediHz.Application.DTOs.Projects;

public enum AddProjectMemberStatus
{
    Created,
    Duplicate,
    OwnerTarget,
    ProjectNotFound,
    UserNotFound,
    InvalidRole,
}

public sealed record AddProjectMemberOutcome(
    AddProjectMemberStatus Status,
    ProjectMemberDto? Member = null
);
