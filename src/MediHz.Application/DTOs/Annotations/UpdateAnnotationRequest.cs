namespace MediHz.Application.DTOs.Annotations;

public sealed record UpdateAnnotationRequest(
    string? LabelName,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds
);
