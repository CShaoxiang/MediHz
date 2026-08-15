using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Projects;

public class ProjectService(MediHzDbContext dbContext) : IProjectService
{
    public async Task<ProjectMemberDto> AddMemberAsync(Guid projectId, AddProjectMemberRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto> CreateAsync(Guid ownerId, CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = new Project(request.Name, ownerId);
        await dbContext.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // TODO: User logic
        return new ProjectDto(project.Id, project.Name, project.OwnerId, "Owner", project.CreatedAtUtc);
    }

    public async Task<bool> DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ProjectDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // TODO: User logic
        return await dbContext.Projects
            .Select(p => new ProjectDto(p.Id, p.Name, p.OwnerId, "Owner", p.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectMemberDto>> GetMembersAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectDto?> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectMemberDto?> UpdateMemberRoleAsync(Guid projectId, Guid userId, UpdateProjectMemberRoleRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
