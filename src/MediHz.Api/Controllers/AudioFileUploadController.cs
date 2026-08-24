using MediHz.Application.DTOs.AudioFiles;
using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AudioFileUploadController(IAudioFileUploadHandler audioFileUploadHandler) : ControllerBase
{
    [HttpPost("batch")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BatchAudioFileUploadResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BatchAudioFileUploadResultDto), StatusCodes.Status207MultiStatus)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> UploadBatch(
        [FromForm] List<IFormFile> files,
        CancellationToken cancellationToken)
    {
        var request = new AudioFileUploadRequest(files, cancellationToken);
        var response = await audioFileUploadHandler.Handle(request);

        if (response.Problem != null)
        {
            return BadRequest(response.Problem);
        }

        if (response.StatusCode == StatusCodes.Status201Created)
        {
            return CreatedAtAction(nameof(UploadBatch), new { batchId = response.Result!.BatchId }, response.Result);
        }

        return StatusCode(response.StatusCode, response.Result);
    }
}