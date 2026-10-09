using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.Abstractions.Storage;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Annotations;
using MediHz.Application.Handlers.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public record ProjectDeleteRequest(Guid ProjectId, Guid UserId);

public interface IDeleteProjectHandler : IRequestHandler<ProjectDeleteRequest, ProjectDto?>;

public class DeleteProjectHandler(
    IProjectAccessService access,
    ICurrentUser currentUser,
    IProjectService projectService,
    IAudioFileService audioFileService,
    IAudioFileDeleteHandler audioFileDeleteHandler,
    ILogger<DeleteProjectHandler> logger
) : IDeleteProjectHandler
{
    public async Task<Result<ProjectDto?>> Handle(
        ProjectDeleteRequest request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckProjectAsync(
            request.ProjectId,
            ProjectPermission.Manage,
            cancellationToken
        );
        if (denied is not null)
            return Result<ProjectDto?>.Failure(denied.Title!, denied.Detail!, denied.Status!.Value);

        logger.LogInformation(
            "Deleting project with ID {ProjectId} for user {UserId}",
            request.ProjectId,
            request.UserId
        );

        // Change is user is part of the project and has permission to delete it

        var project = await projectService.GetByIdAsync(
            request.ProjectId,
            currentUser.UserId,
            cancellationToken
        );
        if (project == null)
        {
            logger.LogWarning(
                "Project with ID {ProjectId} not found or user {UserId} does not have permission to delete it",
                request.ProjectId,
                request.UserId
            );
            return Result<ProjectDto?>.Failure(
                "Accessing project failed",
                "Project not found or user does not have permission to delete it",
                StatusCodes.Status404NotFound
            );
        }

        var audioFiles = await audioFileService.GetForProjectAsync(
            request.ProjectId,
            cancellationToken
        );

        if (audioFiles is not null)
        {
            foreach (var audioFile in audioFiles)
            {
                var deleted = await audioFileDeleteHandler.Handle(
                    new AudioFileDeleteRequest(request.ProjectId, audioFile.Id),
                    cancellationToken
                );
                if (deleted.Problem is not null)
                    return Result<ProjectDto?>.Failure(
                        deleted.Problem.Title!,
                        deleted.Problem.Detail!,
                        deleted.StatusCode
                    );
            }
        }

        var projectDeleted = await projectService.DeleteAsync(request.ProjectId, cancellationToken);
        if (!projectDeleted)
        {
            logger.LogWarning(
                "Failed to delete project with ID {ProjectId} for user {UserId}",
                request.ProjectId,
                request.UserId
            );
            return Result<ProjectDto?>.Failure(
                "Deleting project failed",
                "Failed to delete project",
                StatusCodes.Status500InternalServerError
            );
        }

        return Result<ProjectDto?>.Success(null, StatusCodes.Status204NoContent);
    }
}
