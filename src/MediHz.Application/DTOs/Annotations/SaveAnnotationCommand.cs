namespace MediHz.Application.DTOs.Annotations;

public sealed record SaveAnnotationCommand(
    Guid AnnotationId, // Client generated fixed ID
    Guid AudioFileId,
    IReadOnlyList<string> LabelNames,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
