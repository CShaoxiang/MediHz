using MediHz.Domain.Projects;

namespace MediHz.Application.DTOs.Projects;

public sealed record AddProjectMemberRequest(string Email, ProjectMemberRole Role);

public sealed record AddProjectMemberCommand(Guid ProjectId, string Email, ProjectMemberRole Role);
