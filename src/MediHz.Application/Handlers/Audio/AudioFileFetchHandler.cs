using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Domain.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Audio;

public record AudioFileFetchRequest(Guid ProjectId, Guid AudioId);

public interface IAudioFileFetchHandler
    : IRequestHandler<AudioFileFetchRequest, AudioFileFetchResultDto>;

public class AudioFileFetchHandler(
    IAudioFileService audioFileService,
    IAudioFileFetchService audioFileFetchService,
    IProjectService projectService,
    ICurrentUser currentUser
) : IAudioFileFetchHandler
{
    public async Task<Result<AudioFileFetchResultDto>> Handle(
        AudioFileFetchRequest request,
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
            return Result<AudioFileFetchResultDto>.Failure(
                "Project not found",
                "The specified project does not exist."
            );
        
        // Validate audio
        var audio = await audioFileService.GetByIdAsync(request.AudioId,cancellationToken);

        if (audio == null)
            return Result<AudioFileFetchResultDto>.Failure(
                "Audio not found", 
                "The specified audio does not exist."
            );

        if (audio.ProjectId != project.Id)
            return Result<AudioFileFetchResultDto>.Failure("Audio not found", 
            "The specified audio is not part of specified project"
            );

        var audioFormat = audio.Format.ToString();

        var result = await audioFileFetchService.getAudioFileAsync(request.ProjectId,request.AudioId,audioFormat,cancellationToken);
        
        

        return Result<AudioFileFetchResultDto>.Success(result);
    }
}