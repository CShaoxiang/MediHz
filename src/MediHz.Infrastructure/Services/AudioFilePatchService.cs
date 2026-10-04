using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Infrastructure.Persistence;

namespace MediHz.Infrastructure.Services;

public class AudioFilePatchService(MediHzDbContext dbContext) : IAudioFilePatchService
{
    public async Task<AudioFilePatchResultDto> patchAudioFileAsync(
        Guid projectId,
        Guid audioId,
        AudioFilePatchDto changes,
        CancellationToken cancellationToken
    )
    {
        var audio = await dbContext.AudioFiles.FindAsync([audioId], cancellationToken);
        if (audio == null)
            throw new DataMisalignedException($"audio record not found in database");
        if (changes.FileName is not null)
            audio.FileName = changes.FileName;
        if (changes.Channels is not null)
            audio.Channels = changes.Channels.Value;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AudioFilePatchResultDto(audio.Id, audio.FileName);
    }
}
