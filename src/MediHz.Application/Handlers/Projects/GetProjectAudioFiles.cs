using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

using MediHz.Application.Abstractions.Projects;

public record GetProjectAudioFilesRequest(Guid ProjectId);

public interface IGetProjectAudioFilesHandler
    : IRequestHandler<GetProjectAudioFilesRequest, IEnumerable<AudioFileDto>>;

public class GetProjectAudioFilesHandler(
    IProjectAccessService access,
    IAudioFileService audioFileService,
    ILogger<GetProjectAudioFilesHandler> logger
) : IGetProjectAudioFilesHandler
{
    public async Task<Result<IEnumerable<AudioFileDto>>> Handle(
        GetProjectAudioFilesRequest request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckProjectAsync(
            request.ProjectId,
            ProjectPermission.Read,
            cancellationToken
        );
        if (denied is not null)
            return Result<IEnumerable<AudioFileDto>>.Failure(
                denied.Title!,
                denied.Detail!,
                denied.Status!.Value
            );

        logger.LogInformation("Getting files for project with ID {ProjectId}", request.ProjectId);
        var files = await audioFileService.GetForProjectAsync(request.ProjectId, cancellationToken);
        return Result<IEnumerable<AudioFileDto>>.Success(files);
    }
}
