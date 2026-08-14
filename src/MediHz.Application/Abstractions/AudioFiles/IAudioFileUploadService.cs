using MediHz.Application.DTOs.AudioFiles;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFileUploadService
{
    Task<BatchAudioFileUploadResultDto> UploadBatchAsync(
        IReadOnlyList<IFormFile> files,
        CancellationToken cancellationToken
        );
}