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
        var userExists = await dbContext.Users.AnyAsync(u => u.Id == authorId, cancellationToken);

        if (!userExists)
            throw new UnauthorizedAccessException("The authenticated user does not exist.");

        var audioFileExists = await dbContext.AudioFiles.AnyAsync(
            a => a.Id == request.AudioFileId,
            cancellationToken
        );

        if (!audioFileExists)
            throw new KeyNotFoundException("The specified audio file does not exist.");

        if (
            request.LabelNames == null
            || !request.LabelNames.Any(n => !string.IsNullOrWhiteSpace(n))
        )
            throw new ArgumentException("Add at least one tag before saving.");

        var duration = request.EndTimeSeconds - request.StartTimeSeconds;
        if (request.StartTimeSeconds < 0 || duration < 0.1m)
            throw new ArgumentException("Annotations must be at least 0.1 seconds long.");

        var labels = new List<Domain.Annotations.Label>();
        foreach (var labelName in request.LabelNames)
        {
            if (string.IsNullOrWhiteSpace(labelName))
                continue;

            var label = await dbContext.Labels.FirstOrDefaultAsync(
                l => l.Name == labelName,
                cancellationToken
            );

            if (label == null)
            {
                label = new Domain.Annotations.Label { Name = labelName, OwnerId = authorId };
                await dbContext.Labels.AddAsync(label, cancellationToken);
            }
            labels.Add(label);
        }

        var annotation = new Domain.Annotations.Annotation
        {
            Id = request.Id,
            AudioFileId = request.AudioFileId,
            UserId = authorId,
            Labels = labels,
            StartTimeSeconds = request.StartTimeSeconds,
            EndTimeSeconds = request.EndTimeSeconds,
            DurationSeconds = duration,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };

        await dbContext.Annotations.AddAsync(annotation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = await GetByIdAsync(annotation.Id, cancellationToken);
        return dto ?? throw new InvalidOperationException("Failed to retrieve created annotation.");
    }

    public async Task<bool> DeleteAsync(
        Guid annotationId,
        CancellationToken cancellationToken = default
    )
    {
        var annotation = await dbContext.Annotations.FirstOrDefaultAsync(
            a => a.Id == annotationId,
            cancellationToken
        );

        if (annotation == null)
            return false;

        dbContext.Annotations.Remove(annotation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
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
                a.Labels.Select(l => l.Name).ToList(),
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
                a.Labels.Select(l => l.Name).ToList(),
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
        var annotation = await dbContext
            .Annotations.Include(a => a.Labels)
            .FirstOrDefaultAsync(a => a.Id == annotationId, cancellationToken);

        if (annotation == null)
            return null;

        if (
            request.LabelNames == null
            || !request.LabelNames.Any(n => !string.IsNullOrWhiteSpace(n))
        )
            throw new ArgumentException("Add at least one tag before saving.");

        var duration = request.EndTimeSeconds - request.StartTimeSeconds;
        if (request.StartTimeSeconds < 0 || duration < 0.1m)
            throw new ArgumentException("Annotations must be at least 0.1 seconds long.");

        annotation.StartTimeSeconds = request.StartTimeSeconds;
        annotation.EndTimeSeconds = request.EndTimeSeconds;
        annotation.DurationSeconds = duration;
        annotation.UpdatedAtUtc = DateTime.UtcNow;

        annotation.Labels.Clear();
        foreach (var labelName in request.LabelNames)
        {
            if (string.IsNullOrWhiteSpace(labelName))
                continue;

            var label = await dbContext.Labels.FirstOrDefaultAsync(
                l => l.Name == labelName,
                cancellationToken
            );

            if (label == null)
            {
                label = new Domain.Annotations.Label
                {
                    Name = labelName,
                    OwnerId = annotation.UserId,
                };
                await dbContext.Labels.AddAsync(label, cancellationToken);
            }

            annotation.Labels.Add(label);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(annotation.Id, cancellationToken);
    }
}
