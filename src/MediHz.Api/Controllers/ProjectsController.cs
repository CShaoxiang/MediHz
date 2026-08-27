using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(IProjectService projectService, IAudioFileService audioFileService, ILogger<ProjectsController> logger) : ControllerBase
{
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(CancellationToken ct)
    {
        logger.LogInformation("Getting projects for user");
        // TODO: User behaviour
        var projects = await projectService.GetForUserAsync(Guid.NewGuid(), ct);
        return Ok(projects);
    }

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest request, CancellationToken ct)
    {
        logger.LogInformation("Creating a new project");
        // TODO: Owner ID
        var project = await projectService.CreateAsync(Guid.NewGuid(), request, ct);
        return Ok(project);
    }

    [HttpGet("{projectId}/files")]
    [ProducesResponseType(typeof(IEnumerable<AudioFileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiles(Guid projectId, CancellationToken ct)
    {
        logger.LogInformation("Getting files for project with ID {ProjectId}", projectId);
        var files = await audioFileService.GetForProjectAsync(projectId, ct);
        return Ok(files);
    }
}
