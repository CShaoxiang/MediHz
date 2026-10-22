using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

[Authorize]
public sealed class ProjectPresenceHub(
    IProjectAccessService access,
    ICurrentUser currentUser,
    ProjectPresenceTracker presence
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (!Guid.TryParse(Context.GetHttpContext()?.Request.Query["projectId"], out var projectId))
            throw new HubException("Invalid project ID.");
        var denied = await access.CheckProjectAsync(
            projectId, ProjectPermission.Read, Context.ConnectionAborted);
        if (denied is not null)
            throw new HubException(denied.Detail);

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(projectId));
        var snapshot = presence.Connect(Context.ConnectionId, projectId, currentUser.UserId);
        await Clients.Group(GroupName(projectId)).SendAsync("PresenceChanged", snapshot);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var snapshot = presence.Disconnect(Context.ConnectionId);
        if (snapshot is not null)
            await Clients.Group(GroupName(snapshot.ProjectId)).SendAsync("PresenceChanged", snapshot);
        await base.OnDisconnectedAsync(exception);
    }

    private static string GroupName(Guid projectId) => $"presence:{projectId}";
}
