using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Exports;
using MediHz.Application.DTOs.Exports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Exports;

public interface IExportFileHandler : IRequestHandler<ExportFileRequest, FileExportDto>;

public class ExportFileHandler(
    IExportService exportService,
    ICurrentUser currentUser,
    ILogger<ExportFileHandler> logger
) : IExportFileHandler
{
    public async Task<Result<FileExportDto>> Handle(
        ExportFileRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Exporting File");
        var fileExportDto = await exportService.ExportFileAsync(
            request.fileId,
            currentUser.UserId,
            cancellationToken
        );

        if (fileExportDto == null)
        {
            return Result<FileExportDto>.Failure(
                "File Not Found",
                $"Audio file with ID '{request.fileId}' was not found or access was denied.",
                StatusCodes.Status404NotFound
            );
        }

        return Result<FileExportDto>.Success(fileExportDto);
    }
}
