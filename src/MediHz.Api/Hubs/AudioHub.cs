using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

[Authorize]
public class AudioHub(
    ISaveAnnotationHandler saveAnnotationHandler,
    IUpdateAnnotationHandler updateAnnotationHandler,
    IDeleteAnnotationHandler deleteAnnotationHandler,
    IProjectService projectService,
    ICurrentUser currentUser
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var audioId = Context.GetHttpContext()?.Request.Query["audioId"].ToString();
        if (
            Guid.TryParse(audioId, out var audioFileId)
            && await projectService.CanAccessAudioFileAsync(audioFileId, currentUser.UserId)
        )
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, audioFileId.ToString());
        }

        await base.OnConnectedAsync();
    }

    public async Task<ProblemDetails?> SaveAnnotation(SaveAnnotationCommand saveAnnotationCommand)
    {
        var result = await saveAnnotationHandler.Handle(saveAnnotationCommand, default);
        if (result.Value is null)
            return result.Problem;

        await Clients
            .OthersInGroup(saveAnnotationCommand.AudioFileId.ToString())
            .SendAsync("ReceiveAnnotationUpdate", result.Value);
        return null;
    }

    public async Task<ProblemDetails?> UpdateAnnotation(
        UpdateAnnotationCommand updateAnnotationCommand
    )
    {
        var result = await updateAnnotationHandler.Handle(updateAnnotationCommand, default);
        if (result.Value is null)
            return result.Problem;

        await Clients
            .OthersInGroup(updateAnnotationCommand.AudioFileId.ToString())
            .SendAsync("ReceiveAnnotationUpdate", result.Value);
        return null;
    }

    public async Task<ProblemDetails?> DeleteAnnotation(Guid annotationId, Guid audioId)
    {
        var command = new DeleteAnnotationCommand(annotationId, audioId);
        var result = await deleteAnnotationHandler.Handle(command, default);

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
}
