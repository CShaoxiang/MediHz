namespace MediHz.Application.DTOs.Projects;

public sealed record ProjectPatchDto(
    Guid? Id = null,
    string? Name = null
);