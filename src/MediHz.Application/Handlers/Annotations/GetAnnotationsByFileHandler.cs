using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.Annotations;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Annotations;

public sealed record GetAnnotationsByFileRequest(Guid AudioFileId);

public interface IGetAnnotationsByFileHandler
    : IRequestHandler<GetAnnotationsByFileRequest, IEnumerable<AnnotationDto>>;

public class GetAnnotationsByFileHandler(
    IAnnotationService annotationService,
    IAudioFileService audioFileService,
    ILogger<GetAnnotationsByFileHandler> logger
) : IGetAnnotationsByFileHandler
{
    public async Task<Result<IEnumerable<AnnotationDto>>> Handle(
        GetAnnotationsByFileRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting  annotations for file {FileId}", request.AudioFileId);
        var audioFile = await audioFileService.GetByIdAsync(request.AudioFileId);
        if (audioFile is null)
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
