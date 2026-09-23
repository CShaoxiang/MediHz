using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFileFetchService
{
    Task<AudioFileFetchResultDto> getAudioFileAsync(
        Guid projectId,
        Guid audioId,
        string audioFormat,
        CancellationToken cancellationToken
    );
}