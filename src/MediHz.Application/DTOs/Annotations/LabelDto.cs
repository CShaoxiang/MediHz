namespace MediHz.Application.DTOs.Annotations;

public sealed record LabelDto(
    Guid Id,
    string Name,
    Guid? LabelGroupId,
    Guid? OwnerId,
    DateTimeOffset CreatedAtUtc
);
