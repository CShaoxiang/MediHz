using MediHz.Domain.Audio;

namespace MediHz.Application.DTOs.AudioFiles;

public sealed record AudioFilePatchResultDto(
    Guid Id,
    string FileName
);