using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController(
    IGetProjectsHandler getProjectsHandler,
    ICreateProjectHandler createProjectHandler,
    IGetProjectAudioFilesHandler getProjectAudioFilesHandler,
    IGetProjectSingleAudioHandler getProjectSingleAudio
) : ControllerBase
{
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        var response = await getProjectsHandler.Handle(new(), cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateProject(
        [FromForm] CreateProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await createProjectHandler.Handle(request, cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpGet("{projectId}/files")]
    [ProducesResponseType(typeof(IEnumerable<AudioFileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectAudioFiles(
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var response = await getProjectAudioFilesHandler.Handle(new(projectId), cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpGet("{projectId}/{audioId}")]
    [ProducesResponseType(typeof(IEnumerable<AudioFileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleProjectAudio(
        Guid projectId,
        Guid audioId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProjectSingleAudioRequest(projectId, audioId);
        var response = await getProjectSingleAudio.Handle(request,cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }
}
