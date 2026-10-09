using Microsoft.AspNetCore.Mvc;

namespace MediHz.Application.Abstractions.Projects;

public enum ProjectPermission
{
    Read,
    Write,
    Manage,
}

public interface IProjectAccessService
{
    Task<ProblemDetails?> CheckProjectAsync(
        Guid projectId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    );
    Task<ProblemDetails?> CheckAudioAsync(
        Guid audioId,
        Guid? projectId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    );
    Task<ProblemDetails?> CheckAnnotationAsync(
        Guid annotationId,
        Guid audioId,
        ProjectPermission permission,
        CancellationToken cancellationToken
    );
}
