using MediHz.Domain.Audio;

namespace MediHz.Application.DTOs.AudioFiles;

public sealed record AudioFileDto(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    Guid UploaderId,
    string UploaderDisplayName,
    string FileName,
    AudioFileFormat Format,
    decimal Duration
);
