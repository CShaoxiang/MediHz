using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public sealed record GetAnnotationsByFileRequest(Guid AudioFileId);

public interface IGetAnnotationsByFileHandler
    : IRequestHandler<GetAnnotationsByFileRequest, IEnumerable<AnnotationDto>>;

public class GetAnnotationsByFileHandler(
    IAnnotationService annotationService,
    IProjectService projectService,
    ICurrentUser currentUser,
    ILogger<GetAnnotationsByFileHandler> logger
) : IGetAnnotationsByFileHandler
{
    public async Task<Result<IEnumerable<AnnotationDto>>> Handle(
        GetAnnotationsByFileRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting  annotations for file {FileId}", request.AudioFileId);
        var canAccess = await projectService.CanAccessAudioFileAsync(
            request.AudioFileId,
            currentUser.UserId,
            cancellationToken
        );
        if (!canAccess)
        {
            return Result<IEnumerable<AnnotationDto>>.Failure(
                "File not found",
                "The specified  audio file does not exist."
            );
        }
        var annotations = await annotationService.GetForAudioFileAsync(
            request.AudioFileId,
            cancellationToken
        );
        return Result<IEnumerable<AnnotationDto>>.Success(annotations);
    }
}
