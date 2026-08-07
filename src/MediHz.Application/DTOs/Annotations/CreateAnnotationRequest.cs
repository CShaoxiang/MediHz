namespace MediHz.Application.DTOs.Annotations;

public sealed record CreateAnnotationRequest(
    Guid AudioFileId,
    string? LabelName,
    decimal StartTimeSeconds,
    decimal EndTimeSeconds);
