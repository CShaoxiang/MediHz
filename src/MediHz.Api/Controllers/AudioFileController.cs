using MediHz.Application.Handlers.Audio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AudioFileController(
        IAudioFileFetchHandler audioFileFetchHandler
): ControllerBase
{
    [HttpGet("{projectId:guid}/{audioId:guid}")]
    public async Task<IActionResult> FetchAudioFile(
        Guid projectId,
        Guid audioId,
        CancellationToken cancellationToken
    )
    {
        var request = new AudioFileFetchRequest(projectId,audioId);
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
}