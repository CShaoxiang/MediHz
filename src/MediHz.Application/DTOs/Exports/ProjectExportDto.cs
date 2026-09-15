using System.Text.Json.Serialization;

namespace MediHz.Application.DTOs.Exports;

public sealed record ProjectExportDto(
    [property: JsonPropertyName("project_id")] Guid ProjectId,
    [property: JsonPropertyName("project_name")] string ProjectName,
    [property: JsonPropertyName("annotator_ids")] Guid[]? AnnotatorIds,
    [property: JsonPropertyName("files")] FileExportDto[] Files
);
