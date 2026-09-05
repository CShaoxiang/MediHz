using MediHz.Application.DTOs.Annotations;

namespace MediHz.Application.Abstractions.Annotations;

public interface IAnnotationService
{
    Task<AnnotationDto?> GetByIdAsync(
        Guid annotationId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AnnotationDto>> GetForAudioFileAsync(
        Guid audioFileId,
        CancellationToken cancellationToken = default
    );

    Task<AnnotationDto> CreateAsync(
        Guid authorId,
        CreateAnnotationRequest request,
        CancellationToken cancellationToken = default
    );

    Task<AnnotationDto?> UpdateAsync(
        Guid annotationId,
        UpdateAnnotationRequest request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid annotationId, CancellationToken cancellationToken = default);
}
