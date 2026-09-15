using System.Text.Json.Serialization;

namespace MediHz.Application.DTOs.Exports;

public sealed record FileExportDto(
    [property: JsonPropertyName("file_id")] Guid FileId,
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("file_type")] string? FileType,
    [property: JsonPropertyName("duration_seconds")] decimal DurationSeconds,
    [property: JsonPropertyName("sample_rate")] decimal SampleRate,
    [property: JsonPropertyName("channels")] int? Channels,
    [property: JsonPropertyName("annotations")] AnnotationExportDto[] Annotations
);
