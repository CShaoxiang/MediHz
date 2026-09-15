using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Annotations;

public class AnnotationService(MediHzDbContext dbContext) : IAnnotationService
{
    public async Task<AnnotationDto> CreateAsync(
        Guid authorId,
        CreateAnnotationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(
        Guid annotationId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<AnnotationDto?> GetByIdAsync(
        Guid annotationId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Annotations.Where(a => a.Id == annotationId)
            .Select(a => new AnnotationDto(
                a.Id,
                a.AudioFileId,
                a.UserId,
                a.User.DisplayName,
                a.Label.Name,
                a.StartTimeSeconds,
                a.EndTimeSeconds,
                a.DurationSeconds,
                a.CreatedAtUtc,
                a.UpdatedAtUtc ?? a.CreatedAtUtc
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnnotationDto>> GetForAudioFileAsync(
        Guid audioFileId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Annotations.Where(a => a.AudioFileId == audioFileId)
            .Select(a => new AnnotationDto(
                a.Id,
                a.AudioFileId,
                a.UserId,
                a.User.DisplayName,
                a.Label.Name,
                a.StartTimeSeconds,
                a.EndTimeSeconds,
                a.DurationSeconds,
                a.CreatedAtUtc,
                a.UpdatedAtUtc ?? a.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<AnnotationDto?> UpdateAsync(
        Guid annotationId,
        UpdateAnnotationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
