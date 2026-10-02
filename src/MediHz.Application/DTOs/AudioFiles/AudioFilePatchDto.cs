namespace MediHz.Application.DTOs.AudioFiles;

public sealed record AudioFilePatchDto(
    string? FileName = null,
    int? Channels = null
);