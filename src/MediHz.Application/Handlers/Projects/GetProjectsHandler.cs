using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public interface IGetProjectsHandler : IRequestHandler<EmptyRequest, IEnumerable<ProjectDto>>;

public class GetProjectsHandler(IProjectService projectService, ICurrentUser currentUser, ILogger<GetProjectsHandler> logger) : IGetProjectsHandler
{
    public async Task<Result<IEnumerable<ProjectDto>>> Handle(EmptyRequest _, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting projects for user.");
   
        var projects = await projectService.GetForUserAsync(currentUser.UserId, cancellationToken);
        return Result<IEnumerable<ProjectDto>>.Success(projects);
    }
}
