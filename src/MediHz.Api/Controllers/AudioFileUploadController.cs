using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AudioFileUploadController(IAudioFileUploadService audioFileUploadService) : ControllerBase
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
        if (files.Count == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "No files uploaded",
                Detail = "At least one audio file must be supplied in the request.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var result = await audioFileUploadService.UploadBatchAsync(files, cancellationToken);
        var successCount = result.Items.Count(i => i.Status == Status.Success);
        var failureCount = result.Items.Count - successCount;
        if (failureCount > 0 && successCount > 0)
        {
            return StatusCode(StatusCodes.Status207MultiStatus, result);
        }
        
        return CreatedAtAction(nameof(UploadBatch), new { batchId = result.BatchId }, result);
    }
}