using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers.Annotations;
using MediHz.Application.Handlers.Audio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AudioFileController(
        IAudioFileFetchHandler audioFileFetchHandler,
        IAudioFilePatchHandler audioFilePatchHandler,
        IAudioFileDeleteHandler audioFileDeleteHandler,
        IGetAnnotationsByFileHandler getAnnotationsHandler
) : ControllerBase
{
    [HttpGet("{projectId:guid}/{audioId:guid}")]
    public async Task<IActionResult> FetchAudioFile(
        Guid projectId,
        Guid audioId,
        CancellationToken cancellationToken
    )
    {
        var request = new AudioFileFetchRequest(projectId, audioId);
        var response = await audioFileFetchHandler.Handle(request, cancellationToken);

        if (response.Problem != null)
        {
            return StatusCode(response.StatusCode, response.Problem);
        }

        // Return the stream and audio data to be sent over as a FileStreamResult
        // File seems to only to be usable in controllers
        Response.StatusCode = response.StatusCode;
        return  File(response.Value.stream, response.Value.contentType, response.Value.fileName);
    }

    [HttpPatch("{projectId:guid}/{audioId:guid}")]
    [ProducesResponseType(typeof(AudioFileRenameResultDto),StatusCodes.Status200OK)]
    public async Task<IActionResult> PatchAudioFile(
        Guid projectId,
        Guid audioId,
        [FromBody] AudioFilePatchDto changes,
        CancellationToken cancellationToken
    )
    {
        var request = new AudioFilePatchRequest(projectId, audioId, changes);
        var response = await audioFilePatchHandler.Handle(request, cancellationToken);

        return StatusCode(response.StatusCode, response.Body);
    }

    [HttpDelete("{projectId:guid}/{audioId:guid}")]
    [ProducesResponseType(typeof(AudioFileRenameResultDto),StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAudioFile(
        Guid projectId,
        Guid audioId,
        CancellationToken cancellationToken
    )
    {
        var request = new AudioFileDeleteRequest(projectId, audioId);
        var response = await audioFileDeleteHandler.Handle(request,cancellationToken);

        return StatusCode(response.StatusCode,response.Body);
    }

    [HttpGet("{audioId:guid}/annotations")]
    public async Task<IActionResult> GetAnnotations(
        Guid audioId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetAnnotationsByFileRequest(audioId);
        var response = await getAnnotationsHandler.Handle(request, cancellationToken);

        if (response.Problem != null)
        {
            return StatusCode(response.StatusCode, response.Problem);
        }

        return Ok(response.Value);
    }
}
