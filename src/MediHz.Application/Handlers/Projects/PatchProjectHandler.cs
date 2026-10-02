using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.Extensions.Logging;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Abstractions.Projects;
using Microsoft.AspNetCore.Http;

namespace MediHz.Application.Handlers.Projects;

public record ProjectPatchRequest(Guid ProjectId, Guid UserId, UpdateProjectRequest Change);
public interface IPatchProjectHandler : IRequestHandler<ProjectPatchRequest, ProjectDto>;

public class PatchProjectHandler( IProjectService projectService,
    ILogger<IPatchProjectHandler> logger
) : IPatchProjectHandler
{
    public async Task<Result<ProjectDto>> Handle(
        ProjectPatchRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Patching project with ID {ProjectId}", request.ProjectId);

        var change = await projectService.UpdateAsync(request.ProjectId, request.Change, cancellationToken);

        if (change is null)
        {
            return Result<ProjectDto>.Failure("Failed to patch project", "Project Not Found", StatusCodes.Status404NotFound);
        }

        return Result<ProjectDto>.Success(change);
    }
}