using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Projects;

public class AudioFileService(MediHzDbContext dbContext) : IAudioFileService
{
    public async Task<AudioFileDto> CreateAsync(
        Guid projectId,
        Guid uploaderId,
        UploadAudioFileRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(
        Guid audioFileId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<AudioFileDto?> GetByIdAsync(
        Guid audioFileId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .AudioFiles.Where(a => a.Id == audioFileId)
            .Select(a => new AudioFileDto(
                a.Id,
                a.ProjectId,
                a.Project.Name,
                Guid.NewGuid(),
                "UploaderDisplayName",
                a.FileName,
                a.Format,
                a.Duration,
                a.SampleRate,
                a.Channels
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AudioFileDto>> GetForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        // TODO: User implementation
        return await dbContext
            .AudioFiles.Where(x => x.ProjectId == projectId)
            .Select(a => new AudioFileDto(
                a.Id,
                a.ProjectId,
                a.Project.Name,
                Guid.NewGuid(),
                "UploaderDisplayName",
                a.FileName,
                a.Format,
                a.Duration,
                a.SampleRate,
                a.Channels
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<AudioFileDto?> UpdateAsync(
        Guid audioFileId,
        UpdateAudioFileRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
