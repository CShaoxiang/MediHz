using System.ComponentModel.DataAnnotations;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.Abstractions.Users;
using MediHz.Application.DTOs.Projects;
using MediHz.Domain.Projects;
using Microsoft.Extensions.Logging;

namespace MediHz.Application.Handlers.Projects;

public interface IAddProjectMemberHandler
    : IRequestHandler<AddProjectMemberCommand, ProjectMemberDto>;

public class AddProjectMemberHandler(
    IProjectService projectService,
    IProjectAccessService access,
    IUserService userService,
    ILogger<AddProjectMemberHandler> logger
) : IAddProjectMemberHandler
{
    public async Task<Result<ProjectMemberDto>> Handle(
        AddProjectMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var denied = await access.CheckProjectAsync(
            command.ProjectId,
            ProjectPermission.Manage,
            cancellationToken
        );
        if (denied is not null)
            return new(denied.Status!.Value, null, denied);

        var email = command.Email?.Trim().ToLowerInvariant();
        if (
            string.IsNullOrWhiteSpace(email)
            || email.Length > 320
            || email.Any(char.IsWhiteSpace)
            || !new EmailAddressAttribute().IsValid(email)
            || !System.Net.Mail.MailAddress.TryCreate(email, out var address)
            || address.Address != email
        )
            return Result<ProjectMemberDto>.Failure(
                "Invalid email",
                "Enter a valid registered user's email address.",
                400
            );
        if (command.Role is not (ProjectMemberRole.Annotator or ProjectMemberRole.Viewer))
            return Result<ProjectMemberDto>.Failure(
                "Invalid role",
                "Choose Annotator or Viewer.",
                400
            );

        var target = await userService.GetByEmailAsync(email, cancellationToken);
        if (target is null)
            return Result<ProjectMemberDto>.Failure(
                "User not found",
                "No registered user has that email address.",
                404
            );

        var outcome = await projectService.AddMemberAsync(
            command.ProjectId,
            target.UserId,
            command.Role,
            cancellationToken
        );
        logger.LogInformation(
            "Add member to project {ProjectId}: {Status}",
            command.ProjectId,
            outcome.Status
        );
        return outcome.Status switch
        {
            AddProjectMemberStatus.Created => Result<ProjectMemberDto>.Success(
                outcome.Member!,
                201
            ),
            AddProjectMemberStatus.Duplicate => Result<ProjectMemberDto>.Failure(
                "Already a member",
                "This user is already a project member.",
                409
            ),
            AddProjectMemberStatus.OwnerTarget => Result<ProjectMemberDto>.Failure(
                "Already the owner",
                "The project owner already has access.",
                409
            ),
            AddProjectMemberStatus.InvalidRole => Result<ProjectMemberDto>.Failure(
                "Invalid role",
                "Choose Annotator or Viewer.",
                400
            ),
            AddProjectMemberStatus.UserNotFound => Result<ProjectMemberDto>.Failure(
                "User not found",
                "The registered user no longer exists.",
                404
            ),
            _ => Result<ProjectMemberDto>.Failure(
                "Project not found",
                "The project no longer exists.",
                404
            ),
        };
    }
}
