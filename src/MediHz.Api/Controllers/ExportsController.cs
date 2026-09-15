using MediHz.Application.DTOs.Exports;
using MediHz.Application.Handlers.Exports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExportsController(
    IExportProjectHandler exportProjectHandler,
    IExportFileHandler exportFileHandler
) : ControllerBase
{
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ProjectExportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportProject(
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var response = await exportProjectHandler.Handle(new(projectId), cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpGet("file/{fileId}")]
    [ProducesResponseType(typeof(FileExportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportFile(Guid fileId, CancellationToken cancellationToken)
    {
        var response = await exportFileHandler.Handle(new(fileId), cancellationToken);
        return StatusCode(response.StatusCode, response.Body);
    }
}
