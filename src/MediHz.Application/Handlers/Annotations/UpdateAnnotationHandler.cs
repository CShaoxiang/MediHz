using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IUpdateAnnotationHandler : IRequestHandler<UpdateAnnotationCommand, AnnotationDto>;

public class UpdateAnnotationHandler(
    IAnnotationService annotationService,
    ICurrentUser currentUser,
    ILogger<UpdateAnnotationHandler> logger
) : IUpdateAnnotationHandler
{
    public async Task<Result<AnnotationDto>> Handle(
        UpdateAnnotationCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Updating an annotation via Hub request");

        var updateRequest = new UpdateAnnotationRequest(
            request.LabelNames,
            request.StartTimeSeconds,
            request.EndTimeSeconds
        );

        try
        {
            var annotation = await annotationService.UpdateAsync(
                request.AnnotationId,
                updateRequest,
                cancellationToken
            );

            if (annotation is null)
            {
                return Result<AnnotationDto>.Failure(
                    "Annotation Not Found",
                    "The specified annotation does not exist.",
                    StatusCodes.Status404NotFound
                );
            }

            return Result<AnnotationDto>.Success(annotation, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update annotation.");
            return Result<AnnotationDto>.Failure(
                "Annotation Update Failed",
                ex.Message,
                StatusCodes.Status400BadRequest
            );
        }
    }
}
