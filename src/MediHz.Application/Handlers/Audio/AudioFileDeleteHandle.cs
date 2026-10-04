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
    IAudioFileService audioFileService,
    IAudioFileDeleteService audioFileDeleteService,
    IAnnotationService annotationService,
    IDeleteAnnotationHandler annotationDeleteHandler,
    IProjectService projectService,
    ICurrentUser currentUser
) : IAudioFileDeleteHandler
{
    public async Task<Result<AudioFileDeleteResultDto>> Handle(
        AudioFileDeleteRequest request,
        CancellationToken cancellationToken
    )
    {
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

        //delete all annotations for the audio file

        var annotations = await annotationService.GetForAudioFileAsync(
            request.AudioId,
            cancellationToken
        );

        if (annotations is not null)
        {
            await Task.WhenAll(
                annotations
                    .ToList()
                    .Select(async annotation =>
                    {
                        await annotationDeleteHandler.Handle(
                            new DeleteAnnotationCommand(annotation.Id, request.AudioId),
                            cancellationToken
                        );
                    })
            );
        }
        return Result<AudioFileDeleteResultDto>.Success(result);
    }
}
