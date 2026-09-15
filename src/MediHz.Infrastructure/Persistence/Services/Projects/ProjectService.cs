using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Projects;

public class ProjectService(MediHzDbContext dbContext) : IProjectService
{
    public async Task<ProjectMemberDto> AddMemberAsync(
        Guid projectId,
        AddProjectMemberRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto> CreateAsync(
        Guid ownerId,
        CreateProjectRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var ownerExists = await dbContext.Users.AnyAsync(
            user => user.Id == ownerId,
            cancellationToken
        );

        if (!ownerExists)
            throw new UnauthorizedAccessException("The authenticated user does not exist.");

        var project = new Project(request.Name, ownerId);
        await dbContext.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ProjectDto(
            project.Id,
            project.Name,
            project.OwnerId,
            "Owner",
            project.CreatedAtUtc
        );
    }

    public async Task<bool> DeleteAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto?> GetByIdAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Projects.Where(project =>
                project.Id == projectId
                && (project.OwnerId == userId || project.Members.Any(m => m.UserId == userId))
            )
            .Select(project => new ProjectDto(
                project.Id,
                project.Name,
                project.OwnerId,
                "Owner",
                project.CreatedAtUtc
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Projects.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Select(p => new ProjectDto(p.Id, p.Name, p.OwnerId, "Owner", p.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectMemberDto>> GetMembersAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .ProjectMembers.Where(m => m.ProjectId == projectId)
            .Select(m => new ProjectMemberDto(
                m.ProjectId,
                m.UserId,
                m.User.Email,
                m.User.DisplayName,
                m.Role,
                m.JoinedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RemoveMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto?> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectMemberDto?> UpdateMemberRoleAsync(
        Guid projectId,
        Guid userId,
        UpdateProjectMemberRoleRequest request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
