using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.DTOs.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public interface ISaveAnnotationHandler : IRequestHandler<SaveAnnotationCommand, AnnotationDto>;

public class SaveAnnotationHandler(
    IAnnotationService annotationService,
    ICurrentUser currentUser,
    ILogger<SaveAnnotationHandler> logger
) : ISaveAnnotationHandler
{
    public async Task<Result<AnnotationDto>> Handle(
        SaveAnnotationCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Creating a new annotation via Hub request.");

        // Map the Hub request to the standard Create request expected by the service
        var createRequest = new CreateAnnotationRequest(
            request.AudioFileId,
            request.LabelNames,
            request.StartTimeSeconds,
            request.EndTimeSeconds
        );

        try
        {
            var annotation = await annotationService.CreateAsync(
                currentUser.UserId,
                createRequest,
                cancellationToken
            );
            return Result<AnnotationDto>.Success(annotation, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create annotation.");
            return Result<AnnotationDto>.Failure(
                "Annotation Creation Failed",
                ex.Message,
                StatusCodes.Status400BadRequest
            );
        }
    }
}
