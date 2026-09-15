using System.Text.Json.Serialization;

namespace MediHz.Application.DTOs.Exports;

public sealed record AnnotationExportDto(
    [property: JsonPropertyName("annotation_id")] Guid AnnotationId,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("start_time")] decimal StartTime,
    [property: JsonPropertyName("end_time")] decimal EndTime,
    [property: JsonPropertyName("annotator_id")] Guid AnnotatorId
);
