using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.DTOs.AudioFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.Handlers;

public record AudioFileUploadRequest(List<IFormFile> Files, CancellationToken CancellationToken = default);

public record AudioFileUploadResponse(
    int StatusCode, 
    BatchAudioFileUploadResultDto? Result = null, 
    ProblemDetails? Problem = null);

public interface IAudioFileUploadHandler : IRequestHandler<AudioFileUploadRequest, AudioFileUploadResponse>;

public class AudioFileUploadHandler(IAudioFileUploadService audioFileUploadService) : IAudioFileUploadHandler
{
    public async Task<AudioFileUploadResponse> Handle(AudioFileUploadRequest request)
    {
        if (request.Files.Count == 0)
        {
            return new AudioFileUploadResponse(
                StatusCodes.Status400BadRequest,
                Problem: new ProblemDetails
                {
                    Title = "No files uploaded",
                    Detail = "At least one audio file must be supplied in the request.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        var result = await audioFileUploadService.UploadBatchAsync(request.Files, request.CancellationToken);
        var successCount = result.Items.Count(i => i.Status == Status.Success);
        var failureCount = result.Items.Count - successCount;
        
        if (successCount == 0)
        {
            return new AudioFileUploadResponse(StatusCodes.Status400BadRequest, Result: result);
        }
        if (failureCount > 0 && successCount > 0)
        {
            return new AudioFileUploadResponse(StatusCodes.Status207MultiStatus, Result: result);
        }
        
        return new AudioFileUploadResponse(StatusCodes.Status201Created, Result: result);
    }
}
