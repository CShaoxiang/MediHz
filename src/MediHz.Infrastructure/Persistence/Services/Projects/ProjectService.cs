using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace MediHz.Infrastructure.Persistence.Services.Projects;

public class ProjectService(MediHzDbContext dbContext) : IProjectService
{
    public async Task<AddProjectMemberOutcome> AddMemberAsync(
        Guid projectId,
        Guid userId,
        ProjectMemberRole role,
        CancellationToken cancellationToken = default
    )
    {
        if (role is not (ProjectMemberRole.Annotator or ProjectMemberRole.Viewer))
            return new(AddProjectMemberStatus.InvalidRole);
        var project = await dbContext.Projects.FindAsync([projectId], cancellationToken);
        if (project is null)
            return new(AddProjectMemberStatus.ProjectNotFound);
        if (project.OwnerId == userId)
            return new(AddProjectMemberStatus.OwnerTarget);
        var user = await dbContext.Users.FindAsync([userId], cancellationToken);
        if (user is null)
            return new(AddProjectMemberStatus.UserNotFound);
        if (await IsMemberExistsAsync(projectId, userId, cancellationToken))
            return new(AddProjectMemberStatus.Duplicate);

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
        };
        dbContext.ProjectMembers.Add(member);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is Npgsql.PostgresException
                    {
                        SqlState: Npgsql.PostgresErrorCodes.UniqueViolation,
                        ConstraintName: "PK_ProjectMembers"
                    }
            )
        {
            // Concurrent submissions can both pass the pre-check; the composite key arbitrates.
            dbContext.Entry(member).State = EntityState.Detached;
            return new(AddProjectMemberStatus.Duplicate);
        }
        return new(
            AddProjectMemberStatus.Created,
            new ProjectMemberDto(
                member.ProjectId,
                member.UserId,
                user.Email,
                user.DisplayName,
                member.Role,
                member.JoinedAt
            )
        );
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
            (await dbContext.Users.FindAsync([project.OwnerId], cancellationToken))!.DisplayName,
            project.CreatedAtUtc,
            ProjectMemberRole.Owner
        );
    }

    public async Task<bool> DeleteAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        var project = await dbContext.Projects.FindAsync([projectId], cancellationToken);
        if (project is null)
            return false;

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
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
                project.Owner.DisplayName,
                project.CreatedAtUtc,
                project.OwnerId == userId
                    ? ProjectMemberRole.Owner
                    : project.Members.Where(m => m.UserId == userId).Select(m => m.Role).First()
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CanAccessAudioFileAsync(
        Guid audioFileId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.AudioFiles.AnyAsync(
            audio =>
                audio.Id == audioFileId
                && (
                    audio.Project.OwnerId == userId
                    || audio.Project.Members.Any(m => m.UserId == userId)
                ),
            cancellationToken
        );
    }

    public async Task<bool> CanAccessAnnotationAsync(
        Guid annotationId,
        Guid audioFileId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Annotations.AnyAsync(
            annotation =>
                annotation.Id == annotationId
                && annotation.AudioFileId == audioFileId
                && (
                    annotation.AudioFile.Project.OwnerId == userId
                    || annotation.AudioFile.Project.Members.Any(m => m.UserId == userId)
                ),
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<ProjectDto>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Projects.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.OwnerId,
                p.Owner.DisplayName,
                p.CreatedAtUtc,
                p.OwnerId == userId
                    ? ProjectMemberRole.Owner
                    : p.Members.Where(m => m.UserId == userId).Select(m => m.Role).First()
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectMemberDto>> GetMembersAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        var members = await dbContext
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
        // Owners have access through Projects.OwnerId and may not have a membership row.
        var owner = await dbContext.Projects.Where(p => p.Id == projectId)
            .Select(p => new ProjectMemberDto(p.Id, p.OwnerId, p.Owner.Email,
                p.Owner.DisplayName, ProjectMemberRole.Owner, p.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
        if (owner is not null)
        {
            members.RemoveAll(m => m.UserId == owner.UserId);
            members.Insert(0, owner);
        }
        return members;
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
        var project = await dbContext.Projects.FindAsync([projectId], cancellationToken);
        if (project is null)
            return null;

        if (request.Name is not null)
            project.Name = request.Name;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ProjectDto(
            project.Id,
            project.Name,
            project.OwnerId,
            (await dbContext.Users.FindAsync([project.OwnerId], cancellationToken))!.DisplayName,
            project.CreatedAtUtc,
            ProjectMemberRole.Owner
        );
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

    public async Task<bool> IsMemberExistsAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ProjectMembers.AnyAsync(
            member => member.ProjectId == projectId && member.UserId == userId,
            cancellationToken
        );
    }
}
