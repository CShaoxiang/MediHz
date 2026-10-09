using MediHz.Domain.Projects;

namespace MediHz.Application.DTOs.Projects;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    Guid OwnerId,
    string OwnerDisplayName,
    DateTimeOffset CreatedAt,
    ProjectMemberRole Role = ProjectMemberRole.Viewer
);
