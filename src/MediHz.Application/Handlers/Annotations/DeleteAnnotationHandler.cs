using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

using MediHz.Application.Abstractions.Projects;

public interface IDeleteAnnotationHandler : IRequestHandler<DeleteAnnotationCommand, bool>;

public class DeleteAnnotationHandler(
    IProjectAccessService access,
    IAnnotationService annotationService,
    IProjectService projectService,
    ICurrentUser currentUser,
    ILogger<DeleteAnnotationHandler> logger
) : IDeleteAnnotationHandler
{
    public async Task<Result<bool>> Handle(
        DeleteAnnotationCommand request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckAnnotationAsync(
            request.AnnotationId,
            request.AudioFileId,
            ProjectPermission.Write,
            cancellationToken
        );
        if (denied is not null)
            return Result<bool>.Failure(denied.Title!, denied.Detail!, denied.Status!.Value);

        logger.LogInformation("Deleting an annotation via Hub request");

        try
        {
            var canAccess = await projectService.CanAccessAnnotationAsync(
                request.AnnotationId,
                request.AudioFileId,
                currentUser.UserId,
                cancellationToken
            );

            var result =
                canAccess
                && await annotationService.DeleteAsync(request.AnnotationId, cancellationToken);

            if (!result)
            {
                return Result<bool>.Failure(
                    "Annotation Not Found",
                    "The specified annotation does not exist.",
                    StatusCodes.Status404NotFound
                );
            }

            return Result<bool>.Success(true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete annotation.");
            return Result<bool>.Failure(
                "Annotation Deletion Failed",
                ex.Message,
                StatusCodes.Status400BadRequest
            );
        }
    }
}
