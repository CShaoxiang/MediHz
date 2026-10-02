using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace MediHz.Infrastructure.Services;

public class AudioFileDeleteService(
    MediHzDbContext dbContext,
    IStorageService storageService
) : IAudioFileDeleteService
{
    public async Task<AudioFileDeleteResultDto> deleteAudioFileAsync(
        Guid projectId,
        Guid audioId,
        string audioFormat,
        CancellationToken cancellationToken
    )
    {

        var audio = await dbContext.AudioFiles.FindAsync(audioId);

        if (audio is not null){
        dbContext.AudioFiles.Remove(audio);
        await dbContext.SaveChangesAsync();
        }

        string relativePath = Path.Combine(projectId.ToString(),$"{audioId}.{audioFormat.ToLower()}");
        await storageService.DeleteFileAsync(relativePath);

        return new AudioFileDeleteResultDto(Status.Success);
    }
}