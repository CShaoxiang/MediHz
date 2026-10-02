using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFileDeleteService
{
    Task<AudioFileDeleteResultDto> deleteAudioFileAsync(
        Guid projectId,
        Guid audioId,
        string audio,
        CancellationToken cancellationToken
    );
}