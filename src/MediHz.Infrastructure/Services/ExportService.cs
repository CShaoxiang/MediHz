using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Abstractions.AudioFiles;
using MediHz.Application.Abstractions.Exports;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.DTOs.Exports;

namespace MediHz.Infrastructure.Services;

public class ExportService(
    IProjectService projectService,
    IAudioFileService audioFileService,
    IAnnotationService annotationService
) : IExportService
{
    public async Task<ProjectExportDto?> ExportProjectAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var project = await projectService.GetByIdAsync(projectId, userId, cancellationToken);
        if (project == null)
        {
            return null;
        }

        var members = await projectService.GetMembersAsync(projectId, cancellationToken);
        var files = await audioFileService.GetForProjectAsync(projectId, cancellationToken);
        var fileExports = new List<FileExportDto>(files.Count);
        foreach (var file in files)
        {
            var fileExport = await ExportFileAsync(file.Id, userId, cancellationToken);
            if (fileExport is not null)
            {
                fileExports.Add(fileExport);
            }
        }

        return new ProjectExportDto(
            projectId,
            project.Name,
            members.Select(member => member.UserId).ToArray(),
            fileExports.ToArray()
        );
    }

    public async Task<FileExportDto?> ExportFileAsync(
        Guid fileId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var file = await audioFileService.GetByIdAsync(fileId, cancellationToken);
        if (file == null)
        {
            return null;
        }

        var project = await projectService.GetByIdAsync(file.ProjectId, userId, cancellationToken);
        if (project == null)
        {
            return null;
        }

        var annotations = await annotationService.GetForAudioFileAsync(fileId, cancellationToken);
        var annotationExports = annotations.Select(ExportAnnotation).ToArray();
        return new FileExportDto(
            fileId,
            file.FileName,
            file.Format.ToString(),
            file.Duration,
            file.SampleRate,
            file.Channels,
            annotationExports
        );
    }

    private static AnnotationExportDto ExportAnnotation(AnnotationDto dto)
    {
        return new AnnotationExportDto(
            dto.Id,
            dto.LabelName ?? "placeholder label",
            dto.StartTimeSeconds,
            dto.EndTimeSeconds,
            dto.AuthorId
        );
    }
}
