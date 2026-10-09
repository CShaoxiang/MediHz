using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public interface IGetProjectsHandler : IRequestHandler<EmptyRequest, IEnumerable<ProjectDto>>
{
    Task<Result<ProjectDto>> GetByIdAsync(Guid projectId, CancellationToken cancellationToken);
}

public class GetProjectsHandler(
    IProjectService projectService,
    ICurrentUser currentUser,
    ILogger<GetProjectsHandler> logger
) : IGetProjectsHandler
{
    public async Task<Result<IEnumerable<ProjectDto>>> Handle(
        EmptyRequest _,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting projects for user.");

        var projects = await projectService.GetForUserAsync(currentUser.UserId, cancellationToken);
        return Result<IEnumerable<ProjectDto>>.Success(projects);
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var project = await projectService.GetByIdAsync(
            projectId,
            currentUser.UserId,
            cancellationToken
        );
        return project is null
            ? Result<ProjectDto>.Failure("Not found", "Project not found or access denied.", 404)
            : Result<ProjectDto>.Success(project);
    }
}
