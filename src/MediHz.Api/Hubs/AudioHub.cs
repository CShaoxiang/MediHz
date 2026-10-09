using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

[Authorize]
public class AudioHub(
    IProjectAccessService access,
    ISaveAnnotationHandler saveAnnotationHandler,
    IUpdateAnnotationHandler updateAnnotationHandler,
    IDeleteAnnotationHandler deleteAnnotationHandler,
    IProjectService projectService,
    ICurrentUser currentUser
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }

    public async Task SendRegionData(string audioId, string regionId)
    {
        var id = await AuthorizeAudio(audioId, ProjectPermission.Write);
        await Clients
            .OthersInGroup(id.ToString())
            .SendAsync("ReceiveRegionData", id.ToString(), regionId);
    }

    public async Task JoinAudioGroup(string audioId)
    {
        var id = await AuthorizeAudio(audioId, ProjectPermission.Read);
        await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
    }

    public async Task LeaveAudioGroup(string audioId)
    {
        if (Guid.TryParse(audioId, out var id))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
    }

    public async Task<Guid?> SaveAnnotation(SaveAnnotationCommand saveAnnotationCommand)
    {
        var result = await saveAnnotationHandler.Handle(
            saveAnnotationCommand,
            Context.ConnectionAborted
        );
        if (result.Problem is not null)
            throw new HubException(result.Problem.Detail);
        if (result.Value is not null)
        {
            await Clients
                .OthersInGroup(saveAnnotationCommand.AudioFileId.ToString())
                .SendAsync("ReceiveAnnotationUpdate", result.Value);
            return result.Value.Id;
        }
        return null;
    }

    public async Task<ProblemDetails?> UpdateAnnotation(
        UpdateAnnotationCommand updateAnnotationCommand
    )
    {
        var result = await updateAnnotationHandler.Handle(
            updateAnnotationCommand,
            Context.ConnectionAborted
        );
        if (result.Problem is not null)
            throw new HubException(result.Problem.Detail);
        if (result.Value is not null)
        {
            await Clients
                .OthersInGroup(updateAnnotationCommand.AudioFileId.ToString())
                .SendAsync("ReceiveAnnotationUpdate", result.Value);
            return result.Value.Id;
        }

        await Clients
            .OthersInGroup(updateAnnotationCommand.AudioFileId.ToString())
            .SendAsync("ReceiveAnnotationUpdate", result.Value);
        return null;
    }

    public async Task<ProblemDetails?> DeleteAnnotation(Guid annotationId, Guid audioId)
    {
        var command = new DeleteAnnotationCommand(annotationId, audioId);
        var result = await deleteAnnotationHandler.Handle(command, Context.ConnectionAborted);
        if (result.Problem is not null)
            throw new HubException(result.Problem.Detail);

        // Already deleted counts as success
        if (result.StatusCode == StatusCodes.Status404NotFound)
            return null;
        if (!result.Value)
            return result.Problem;

        await Clients
            .OthersInGroup(audioId.ToString())
            .SendAsync("ReceiveAnnotationDelete", annotationId);
        return null;
    }

    private async Task<Guid> AuthorizeAudio(string audioId, ProjectPermission permission)
    {
        if (!Guid.TryParse(audioId, out var id))
            throw new HubException("Invalid audio ID.");
        var denied = await access.CheckAudioAsync(id, null, permission, Context.ConnectionAborted);
        if (denied is not null)
            throw new HubException(denied.Detail);
        return id;
    }
}
