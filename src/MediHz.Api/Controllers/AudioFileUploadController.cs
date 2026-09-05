using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers.Audio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AudioFileUploadController(IAudioFileUploadHandler audioFileUploadHandler)
    : ControllerBase
{
    [HttpPost("{projectId:guid}/batch")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BatchAudioFileUploadResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BatchAudioFileUploadResultDto), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadBatch(
        Guid projectId,
        [FromForm] List<IFormFile> files,
        CancellationToken cancellationToken
    )
    {
        var request = new AudioFileUploadRequest(projectId, files);
        var response = await audioFileUploadHandler.Handle(request, cancellationToken);

        return StatusCode(response.StatusCode, response.Body);
    }
}
