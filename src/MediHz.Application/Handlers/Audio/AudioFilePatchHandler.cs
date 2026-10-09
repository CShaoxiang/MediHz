using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;

namespace MediHz.Application.Handlers.Audio;

public record AudioFilePatchRequest(Guid ProjectId, Guid AudioId, AudioFilePatchDto Changes);

public interface IAudioFilePatchHandler
    : IRequestHandler<AudioFilePatchRequest, AudioFilePatchResultDto>;

public class AudioFilePatchHandler(
    IProjectAccessService access,
    IAudioFileService audioFileService,
    IAudioFilePatchService audioFilePatchService,
    IProjectService projectService,
    ICurrentUser currentUser
) : IAudioFilePatchHandler
{
    public async Task<Result<AudioFilePatchResultDto>> Handle(
        AudioFilePatchRequest request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckAudioAsync(
            request.AudioId,
            request.ProjectId,
            ProjectPermission.Write,
            cancellationToken
        );
        if (denied is not null)
            return Result<AudioFilePatchResultDto>.Failure(
                denied.Title!,
                denied.Detail!,
                denied.Status!.Value
            );

        // Validate project
        var project = await projectService.GetByIdAsync(
            request.ProjectId,
            currentUser.UserId,
            cancellationToken
        );

        if (project == null)
            return Result<AudioFilePatchResultDto>.Failure(
                "Project not found",
                "The specified project does not exist."
            );

        // Validate audio
        var audio = await audioFileService.GetByIdAsync(request.AudioId, cancellationToken);

        if (audio == null)
            return Result<AudioFilePatchResultDto>.Failure(
                "Audio not found",
                "The specified audio does not exist."
            );

        if (audio.ProjectId != project.Id)
            return Result<AudioFilePatchResultDto>.Failure(
                "Audio not found",
                "The specified audio is not part of specified project"
            );

        var result = await audioFilePatchService.patchAudioFileAsync(
            request.ProjectId,
            request.AudioId,
            request.Changes,
            cancellationToken
        );

        return Result<AudioFilePatchResultDto>.Success(result);
    }
}
