using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Storage;
using MediHz.Domain.Audio;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Infrastructure.Services;

public class AudioFileUploadService(
    IStorageService storageService,
    MediHzDbContext dbContext,
    ILogger<AudioFileUploadService> logger) : IAudioFileUploadService
{
    public async Task<BatchAudioFileUploadResultDto> UploadBatchAsync(
        IReadOnlyList<IFormFile> files,
        CancellationToken cancellationToken = default)
    {
        var fileResults = new List<AudioFileUploadResultDto>();

        foreach (var file in files)
        {
            //TODO: Add verification for each file
            if (file.Length == 0)
            {
                fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, 0, Status.Failure, "File is empty"));
                continue;
            }
            //TODO: Extract metadata from files and place into AudioFile entry
            using var fileStream = file.OpenReadStream();
            try
            {
                var audioFile = new AudioFile
                {
                    FileName = file.FileName,
                    Format = FromMimeType(file.ContentType.ToLower()),
                    SizeBytes = file.Length,
                    Duration = 10, // Dummy values
                    SampleRate = 44100,
                    Channels = 1
                };
                
                // TODO: Just leaving a note to amend this ones projects are implemented
                string relativePath = Path.Combine(audioFile.ProjectId?.ToString() ?? "exampleProjectId", $"{audioFile.Id}.{audioFile.Format.ToString().ToLower()}");
                relativePath = await storageService.SaveFileAsync(fileStream, relativePath, cancellationToken);
                dbContext.AudioFiles.Add(audioFile);
                
                fileResults.Add(new AudioFileUploadResultDto(audioFile.Id, audioFile.FileName, audioFile.Format.ToString(), audioFile.SizeBytes, Status.Success, null));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload file {FileName}", file.FileName);
                fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, file.Length, Status.Failure, "Internal processing error"));
            }
        }
        // 4. Save all records in one batch
        if (fileResults.Any(i => i.Status == Status.Success))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return new BatchAudioFileUploadResultDto(
            Guid.NewGuid(),
            fileResults
        );
    }

    private static AudioFileFormat FromMimeType(string mimeType)
    {
        return mimeType?.ToLower() switch
        {
            "audio/mpeg" or "audio/mp3" => AudioFileFormat.Mp3,
            "audio/wav" or "audio/x-wav" => AudioFileFormat.Wav,
            _ => throw new ArgumentException($"Unsupported MIME type: {mimeType}", nameof(mimeType))
        };
    }
}