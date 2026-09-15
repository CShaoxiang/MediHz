using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Exports;
using MediHz.Application.DTOs.Exports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Exports;

public interface IExportProjectHandler : IRequestHandler<ExportProjectRequest, ProjectExportDto>;

public class ExportProjectHandler(
    IExportService exportService,
    ICurrentUser currentUser,
    ILogger<ExportProjectHandler> logger
) : IExportProjectHandler
{
    public async Task<Result<ProjectExportDto>> Handle(
        ExportProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Exporting Project");
        var projectExportDto = await exportService.ExportProjectAsync(
            request.projectId,
            currentUser.UserId,
            cancellationToken
        );
        if (projectExportDto == null)
        {
            return Result<ProjectExportDto>.Failure(
                "File Not Found",
                $"Project with ID '{request.projectId}' was not found or access was denied.",
                StatusCodes.Status404NotFound
            );
        }
        return Result<ProjectExportDto>.Success(projectExportDto);
    }
}
