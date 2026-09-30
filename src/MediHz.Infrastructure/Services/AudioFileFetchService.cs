using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.Extensions.Logging;

namespace MediHz.Infrastructure.Services;

public class AudioFileFetchService(
    IStorageService storageService,
    ILogger<AudioFileFetchService> logger
) : IAudioFileFetchService
{
    public async Task<AudioFileFetchResultDto> getAudioFileAsync(
        Guid projectId,
        Guid audioId,
        string audioFormat,
        CancellationToken cancellationToken
    )
    {
        string relativePath = Path.Combine(
            projectId.ToString(),
            $"{audioId}.{audioFormat.ToLower()}"
        );
        var stream = await storageService.GetFileAsync(relativePath, cancellationToken);

        if (stream is null)
        {
            logger.LogError("No file located for Audio Path {AudioPath}", relativePath);
            throw new FileNotFoundException($"Audio file not found at path: {relativePath}");
        }

        var fileName = $"{audioId}.{audioFormat.ToLower()}";
        return new AudioFileFetchResultDto(
            audioId,
            stream,
            audioFormat,
            GetMimeType(audioFormat),
            fileName
        );
    }

    private static string GetMimeType(string audioFormat)
    {
        return audioFormat?.ToLower() switch
        {
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            _ => "application/octet-stream",
        };
    }
}
