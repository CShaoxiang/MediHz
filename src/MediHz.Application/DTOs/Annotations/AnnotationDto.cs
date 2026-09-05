namespace MediHz.Application.DTOs.Annotations;

public sealed record AnnotationDto(
    Guid Id,
    Guid AudioFileId,
    Guid AuthorId,
    string AuthorDisplayName,
    string? LabelName,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds,
    decimal DurationSeconds,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
