using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Storage;
using MediHz.Domain.Audio;
using MediHz.Infrastructure.Persistence;
using MediHz.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace MediHz.Infrastructure.Services;

public class AudioFileUploadService(
    IStorageService storageService,
    MediHzDbContext dbContext,
    ILogger<AudioFileUploadService> logger) : IAudioFileUploadService
{
    private const int MaxDurationSeconds = 1800;
    private const int MinDurationSeconds = 1;
    private const int MinSampleRate = 5000;
    public async Task<BatchAudioFileUploadResultDto> UploadBatchAsync(
        IReadOnlyList<IFormFile> files,
        CancellationToken cancellationToken = default)
    {
        var fileResults = new List<AudioFileUploadResultDto>();

        foreach (var file in files)
        {
            var format = FromMimeType(file.ContentType.ToLower());
            TimeSpan duration;
            int sampleRate;
            int channels;
            
            if (file.Length == 0)
            {
                fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, 0, Status.Failure, "File is empty"));
                continue;
            }
            using var fileStream = file.OpenReadStream();
            try
            {
                using WaveStream waveStream = format switch
                {
                    AudioFileFormat.Wav => new WaveFileReader(fileStream),
                    AudioFileFormat.Mp3 => new Mp3FileReader(fileStream),
                    _ => throw new NotSupportedException("Format not supported for metadata extraction")
                };
                // Extract metadata from audio file
                duration = waveStream.TotalTime;
                sampleRate = waveStream.WaveFormat.SampleRate;
                channels = waveStream.WaveFormat.Channels;

                // Perform verification on audioFile
                if (duration.TotalSeconds > MaxDurationSeconds)
                {
                    logger.LogError("Audio file {FileName} exceeds maximum duration of {MaxDuration}", file.FileName, MaxDurationSeconds);
                    fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, 0, Status.Failure, "File is too long"));
                    continue;
                }
                if (duration.TotalSeconds < MinDurationSeconds)
                {
                    logger.LogError("Audio file {FileName} does not reach minimum duration of {MinDuration}", file.FileName, MinDurationSeconds);
                    fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, 0, Status.Failure, "File is too short"));
                    continue;
                }
                if (sampleRate < MinSampleRate)
                {
                    logger.LogError("Audio file {FileName} does not reach the minimum sample rate of {MinRate}", file.FileName,  MinSampleRate);
                    fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, 0, Status.Failure, "File does not reach minimum sample rate"));
                    continue;
                }
            }catch (Exception ex)
            {
                // 4. Verification fails here if the file is corrupted or not a valid audio file
                logger.LogWarning(ex, "Failed to parse audio headers for file {FileName}", file.FileName);
                fileResults.Add(new AudioFileUploadResultDto(Guid.NewGuid(), file.FileName, null, file.Length, Status.Failure, "Invalid or corrupted audio file"));
                continue;
            }
            
            try
            {
                // Build AudioFile DB entity if all requirements are satisfied
                var audioFile = new AudioFile
                {
                    FileName = file.FileName,
                    Format = format,
                    SizeBytes = file.Length,
                    Duration = (decimal)duration.TotalSeconds, 
                    SampleRate = sampleRate,
                    Channels = channels
                };
                
                // TODO: Just leaving a note to amend this ones projects are implemented
                // Need to reset fileStream position before saving 
                fileStream.Position = 0;
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