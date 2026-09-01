using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public interface ICreateProjectHandler : IRequestHandler<CreateProjectRequest, ProjectDto>;

public class CreateProjectHandler(IProjectService projectService,ICurrentUser currentUser, ILogger<CreateProjectHandler> logger) : ICreateProjectHandler
{
    public async Task<Result<ProjectDto>> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new project.");
  
        var project = await projectService.CreateAsync(currentUser.UserId, request, cancellationToken);
        return Result<ProjectDto>.Success(project, StatusCodes.Status201Created);
    }
}
