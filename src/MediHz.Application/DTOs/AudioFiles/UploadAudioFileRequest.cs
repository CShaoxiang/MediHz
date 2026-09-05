using MediHz.Domain.Audio;

namespace MediHz.Application.DTOs.AudioFiles;

public sealed record UploadAudioFileRequest(
    string FileName,
    AudioFileFormat Format,
    decimal Duration
);
