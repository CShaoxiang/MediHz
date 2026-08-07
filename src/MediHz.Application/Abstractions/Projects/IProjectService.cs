using MediHz.Application.DTOs.Projects;

namespace MediHz.Application.Abstractions.Projects;

public interface IProjectService
{
    /// <summary>
    /// TODO: Return a project with ownership and summary details.
    /// </summary>
    Task<ProjectDto?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Return all projects visible to the current user.
    /// </summary>
    Task<IReadOnlyList<ProjectDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Create a new project for the current owner.
    /// </summary>
    Task<ProjectDto> CreateAsync(Guid ownerId, CreateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Rename a project.
    /// </summary>
    Task<ProjectDto?> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Delete a project and its dependent data when allowed.
    /// </summary>
    Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Return all members for a project.
    /// </summary>
    Task<IReadOnlyList<ProjectMemberDto>> GetMembersAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Add a user to a project.
    /// </summary>
    Task<ProjectMemberDto> AddMemberAsync(Guid projectId, AddProjectMemberRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Update a member's project role.
    /// </summary>
    Task<ProjectMemberDto?> UpdateMemberRoleAsync(Guid projectId, Guid userId, UpdateProjectMemberRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// TODO: Remove a member from a project.
    /// </summary>
    Task<bool> RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
}
