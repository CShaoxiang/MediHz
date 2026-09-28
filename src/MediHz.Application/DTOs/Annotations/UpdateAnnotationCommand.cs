namespace MediHz.Application.DTOs.Annotations;

public sealed record UpdateAnnotationCommand(
    Guid AnnotationId,
    Guid AudioFileId,
    IReadOnlyList<string> LabelNames,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
