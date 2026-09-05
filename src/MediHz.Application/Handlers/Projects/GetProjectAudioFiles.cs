using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public record GetProjectAudioFilesRequest(Guid ProjectId);

public interface IGetProjectAudioFilesHandler
    : IRequestHandler<GetProjectAudioFilesRequest, IEnumerable<AudioFileDto>>;

public class GetProjectAudioFilesHandler(
    IAudioFileService audioFileService,
    ILogger<GetProjectAudioFilesHandler> logger
) : IGetProjectAudioFilesHandler
{
    public async Task<Result<IEnumerable<AudioFileDto>>> Handle(
        GetProjectAudioFilesRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting files for project with ID {ProjectId}", request.ProjectId);
        var files = await audioFileService.GetForProjectAsync(request.ProjectId, cancellationToken);
        return Result<IEnumerable<AudioFileDto>>.Success(files);
    }
}
