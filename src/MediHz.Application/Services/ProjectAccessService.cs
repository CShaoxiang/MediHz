using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Domain.Projects;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.Services;

public class ProjectAccessService(
    IProjectService projects,
    IAudioFileService audioFiles,
    IAnnotationService annotations,
    ICurrentUser currentUser
) : IProjectAccessService
{
    private static ProblemDetails NotFound() =>
        new()
        {
            Status = 404,
            Title = "Not found",
            Detail = "The requested resource was not found or access was denied.",
        };

    public async Task<ProblemDetails?> CheckProjectAsync(
        Guid projectId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    )
    {
        var project = await projects.GetByIdAsync(projectId, currentUser.UserId, cancellationToken);
        if (project is null)
            return NotFound();
        if (project.OwnerId == currentUser.UserId)
            return null;
        if (
            permission == ProjectPermission.Read
            && project.Role is ProjectMemberRole.Annotator or ProjectMemberRole.Viewer
        )
            return null;
        if (permission == ProjectPermission.Write && project.Role == ProjectMemberRole.Annotator)
            return null;
        return new()
        {
            Status = 403,
            Title = "Forbidden",
            Detail = "Your project role does not permit this action.",
        };
    }

    public async Task<ProblemDetails?> CheckAudioAsync(
        Guid audioId,
        Guid? projectId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    )
    {
        var audio = await audioFiles.GetByIdAsync(audioId, cancellationToken);
        if (audio is null || (projectId.HasValue && audio.ProjectId != projectId.Value))
            return NotFound();
        return await CheckProjectAsync(audio.ProjectId, permission, cancellationToken);
    }

    public async Task<ProblemDetails?> CheckAnnotationAsync(
        Guid annotationId,
        Guid audioId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    )
    {
        var annotation = await annotations.GetByIdAsync(annotationId, cancellationToken);
        if (annotation is null || annotation.AudioFileId != audioId)
            return NotFound();
        return await CheckAudioAsync(annotation.AudioFileId, null, permission, cancellationToken);
    }
}
