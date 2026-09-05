using MediHz.Application.DTOs.AudioFiles;

namespace MediHz.Application.Abstractions.AudioFiles;

public interface IAudioFileService
{
    Task<AudioFileDto?> GetByIdAsync(
        Guid audioFileId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AudioFileDto>> GetForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    );

    Task<AudioFileDto> CreateAsync(
        Guid projectId,
        Guid uploaderId,
        UploadAudioFileRequest request,
        CancellationToken cancellationToken = default
    );

    Task<AudioFileDto?> UpdateAsync(
        Guid audioFileId,
        UpdateAudioFileRequest request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid audioFileId, CancellationToken cancellationToken = default);
}
