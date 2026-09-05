using MediHz.Domain.Projects;

namespace MediHz.Application.DTOs.Projects;

public sealed record AddProjectMemberRequest(Guid UserId, ProjectMemberRole Role);
