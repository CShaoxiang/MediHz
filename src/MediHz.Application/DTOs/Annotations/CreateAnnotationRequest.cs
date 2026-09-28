namespace MediHz.Application.DTOs.Annotations;

public sealed record CreateAnnotationRequest(
    Guid AudioFileId,
    IReadOnlyList<string> LabelNames,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
