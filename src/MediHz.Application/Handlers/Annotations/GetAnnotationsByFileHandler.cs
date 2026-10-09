using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

using MediHz.Application.Abstractions.Projects;

public sealed record GetAnnotationsByFileRequest(Guid AudioFileId);

public interface IGetAnnotationsByFileHandler
    : IRequestHandler<GetAnnotationsByFileRequest, IEnumerable<AnnotationDto>>;

public class GetAnnotationsByFileHandler(
    IProjectAccessService access,
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
        var denied = await access.CheckAudioAsync(
            request.AudioFileId,
            null,
            ProjectPermission.Read,
            cancellationToken
        );
        if (denied is not null)
            return Result<IEnumerable<AnnotationDto>>.Failure(
                denied.Title!,
                denied.Detail!,
                denied.Status!.Value
            );

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
