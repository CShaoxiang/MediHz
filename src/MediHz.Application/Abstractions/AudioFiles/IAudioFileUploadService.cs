using MediHz.Application.DTOs.AudioFiles;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFileUploadService
{
    Task<BatchAudioFileUploadResultDto> UploadBatchAsync(
        Guid projectId,
        IReadOnlyList<IFormFile> files,
        CancellationToken cancellationToken
        );
}
