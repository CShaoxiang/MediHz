namespace MediHz.Application.DTOs.Annotations;

public sealed record CreateAnnotationRequest(
    Guid Id,
    Guid AudioFileId,
    IReadOnlyList<string> LabelNames,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
