namespace MediHz.Application.DTOs.Annotations;

public sealed record UpdateAnnotationRequest(
    IReadOnlyList<string> LabelNames,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
