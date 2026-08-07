using MediHz.Domain.Projects;

namespace MediHz.Application.DTOs.Projects;

public sealed record ProjectMemberDto(
    Guid ProjectId,
    Guid UserId,
    string Email,
    string DisplayName,
    ProjectMemberRole Role,
    DateTimeOffset JoinedAt);
