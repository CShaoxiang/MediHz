using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFilePatchService
{
    Task<AudioFilePatchResultDto> patchAudioFileAsync(
        Guid projectId,
        Guid audioId,
        AudioFilePatchDto changes,
        CancellationToken cancellationToken
    );
}