using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers.Annotations;

namespace MediHz.Application.Handlers.Audio;

public record AudioFileDeleteRequest(Guid ProjectId, Guid AudioId);

public interface IAudioFileDeleteHandler
    : IRequestHandler<AudioFileDeleteRequest, AudioFileDeleteResultDto>;

public class AudioFileDeleteHandler(
    IProjectAccessService access,
    IAudioFileService audioFileService,
    IAudioFileDeleteService audioFileDeleteService,
    IProjectService projectService,
    ICurrentUser currentUser
) : IAudioFileDeleteHandler
{
    public async Task<Result<AudioFileDeleteResultDto>> Handle(
        AudioFileDeleteRequest request,
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
            return Result<AudioFileDeleteResultDto>.Failure(
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
            return Result<AudioFileDeleteResultDto>.Failure(
                "Project not found",
                "The specified project does not exist."
            );

        // Validate audio
        var audio = await audioFileService.GetByIdAsync(request.AudioId, cancellationToken);

        if (audio == null)
            return Result<AudioFileDeleteResultDto>.Failure(
                "Audio not found",
                "The specified audio does not exist."
            );

        if (audio.ProjectId != project.Id)
            return Result<AudioFileDeleteResultDto>.Failure(
                "Audio not found",
                "The specified audio is not part of specified project"
            );

        var result = await audioFileDeleteService.deleteAudioFileAsync(
            request.ProjectId,
            request.AudioId,
            audio.Format.ToString(),
            cancellationToken
        );

        // Annotation rows are deleted by the existing AudioFile foreign-key cascade.
        return Result<AudioFileDeleteResultDto>.Success(result);
    }
}
