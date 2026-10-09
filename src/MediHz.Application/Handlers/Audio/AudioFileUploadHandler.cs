using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Handlers.Audio;

public record AudioFileUploadRequest(Guid ProjectId, List<IFormFile> Files);

public interface IAudioFileUploadHandler
    : IRequestHandler<AudioFileUploadRequest, BatchAudioFileUploadResultDto>;

public class AudioFileUploadHandler(
    IProjectAccessService access,
    IProjectService projectService,
    IAudioFileUploadService audioFileUploadService,
    ICurrentUser currentUser
) : IAudioFileUploadHandler
{
    private const int MaxBatchSize = 5;

    public async Task<Result<BatchAudioFileUploadResultDto>> Handle(
        AudioFileUploadRequest request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckProjectAsync(
            request.ProjectId,
            ProjectPermission.Write,
            cancellationToken
        );
        if (denied is not null)
            return Result<BatchAudioFileUploadResultDto>.Failure(
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
            return Result<BatchAudioFileUploadResultDto>.Failure(
                "Project not found",
                "The specified project does not exist."
            );

        if (request.Files.Count == 0)
            return Result<BatchAudioFileUploadResultDto>.Failure(
                "No files uploaded",
                "At least one audio file must be supplied in the request."
            );

        if (request.Files.Count > MaxBatchSize)
        {
            return Result<BatchAudioFileUploadResultDto>.Failure(
                "Maximum batch size exceeded",
                $"Batch size must not exceed {MaxBatchSize}"
            );
        }

        var result = await audioFileUploadService.UploadBatchAsync(
            request.ProjectId,
            request.Files,
            cancellationToken
        );
        var successCount = result.Items.Count(i => i.Status == Status.Success);
        var failureCount = result.Items.Count - successCount;

        if (successCount == 0)
            return Result<BatchAudioFileUploadResultDto>.Failure(
                "Failed to upload files",
                $"Failed to upload all {result.Items.Count} files."
            );

        if (failureCount > 0 && successCount > 0)
            return Result<BatchAudioFileUploadResultDto>.Success(
                result,
                StatusCodes.Status207MultiStatus
            );

        return Result<BatchAudioFileUploadResultDto>.Success(result, StatusCodes.Status201Created);
    }
}
