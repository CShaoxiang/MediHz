using MediHz.Application.DTOs.Exports;

namespace MediHz.Application.Abstractions.Exports;

public interface IExportService
{
    Task<ProjectExportDto?> ExportProjectAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken
    );

    Task<FileExportDto?> ExportFileAsync(
        Guid fileId,
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
