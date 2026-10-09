using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public record ProjectPatchRequest(Guid ProjectId, Guid UserId, UpdateProjectRequest Change);

public interface IPatchProjectHandler : IRequestHandler<ProjectPatchRequest, ProjectDto>;

public class PatchProjectHandler(
    IProjectAccessService access,
    IProjectService projectService,
    ILogger<IPatchProjectHandler> logger
) : IPatchProjectHandler
{
    public async Task<Result<ProjectDto>> Handle(
        ProjectPatchRequest request,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckProjectAsync(
            request.ProjectId,
            ProjectPermission.Manage,
            cancellationToken
        );
        if (denied is not null)
            return Result<ProjectDto>.Failure(denied.Title!, denied.Detail!, denied.Status!.Value);

        logger.LogInformation("Patching project with ID {ProjectId}", request.ProjectId);

        var change = await projectService.UpdateAsync(
            request.ProjectId,
            request.Change,
            cancellationToken
        );

        if (change is null)
        {
            return Result<ProjectDto>.Failure(
                "Failed to patch project",
                "Project Not Found",
                StatusCodes.Status404NotFound
            );
        }

        return Result<ProjectDto>.Success(change);
    }
}
