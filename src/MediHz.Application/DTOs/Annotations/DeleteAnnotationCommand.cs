namespace MediHz.Application.DTOs.Annotations;

public sealed record DeleteAnnotationCommand(Guid AnnotationId, Guid AudioFileId);
