namespace MediHz.Application.DTOs.Annotations;

public sealed record LabelGroupDto(
    Guid Id,
    string Name,
    Guid? OwnerId,
    DateTimeOffset CreatedAtUtc
);
