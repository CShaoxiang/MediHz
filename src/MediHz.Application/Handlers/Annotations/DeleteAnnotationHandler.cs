using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface IDeleteAnnotationHandler : IRequestHandler<DeleteAnnotationCommand, bool>;

public class DeleteAnnotationHandler(
    IAnnotationService annotationService,
    ILogger<DeleteAnnotationHandler> logger
) : IDeleteAnnotationHandler
{
    public async Task<Result<bool>> Handle(
        DeleteAnnotationCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Deleting an annotation via Hub request");

        try
        {
            var result = await annotationService.DeleteAsync(
                request.AnnotationId,
                cancellationToken
            );

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
