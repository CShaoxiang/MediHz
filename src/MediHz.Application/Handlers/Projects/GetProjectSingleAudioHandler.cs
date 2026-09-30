using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public record GetProjectSingleAudioRequest(Guid ProjectId, Guid AudioId);

public interface IGetProjectSingleAudioHandler
    : IRequestHandler<GetProjectSingleAudioRequest, AudioFileDto>;

public class GetProjectSingleAudioHandler(
    IAudioFileService audioFileService,
    ILogger<GetProjectSingleAudioHandler> logger
) : IGetProjectSingleAudioHandler
{
    public async Task<Result<AudioFileDto>> Handle(
        GetProjectSingleAudioRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Getting single file {AudioID} for project with ID {ProjectId}",
            request.AudioId,
            request.ProjectId
        );
        var file = await audioFileService.GetByIdAsync(request.AudioId, cancellationToken);
        if (file is null)
        {
            return Result<AudioFileDto>.Failure("Failed to get audio", "Audio Not Found");
        }
        return Result<AudioFileDto>.Success(file);
    }
}
