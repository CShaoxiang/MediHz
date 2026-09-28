using MediHz.Application.DTOs.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

[Authorize]
public class AudioHub(
    ISaveAnnotationHandler saveAnnotationHandler,
    IUpdateAnnotationHandler updateAnnotationHandler,
    IDeleteAnnotationHandler deleteAnnotationHandler
) : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task SendRegionData(string audioId, string regionId)
    {
        await Clients.OthersInGroup(audioId).SendAsync("ReceiveRegionData", audioId, regionId);
    }

    public async Task JoinAudioGroup(string audioId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, audioId);
    }

    public async Task LeaveAudioGroup(string audioId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, audioId);
    }

    public async Task<Guid?> SaveAnnotation(SaveAnnotationCommand saveAnnotationCommand)
    {
        var result = await saveAnnotationHandler.Handle(saveAnnotationCommand, default);
        if (result.Value is not null)
        {
            await Clients
                .OthersInGroup(saveAnnotationCommand.AudioFileId.ToString())
                .SendAsync("ReceiveAnnotationUpdate", result.Value);
            return result.Value.Id;
        }
        return null;
    }

    public async Task<Guid?> UpdateAnnotation(UpdateAnnotationCommand updateAnnotationCommand)
    {
        var result = await updateAnnotationHandler.Handle(updateAnnotationCommand, default);
        if (result.Value is not null)
        {
            await Clients
                .OthersInGroup(updateAnnotationCommand.AudioFileId.ToString())
                .SendAsync("ReceiveAnnotationUpdate", result.Value);
            return result.Value.Id;
        }

        return null;
    }

    public async Task<bool> DeleteAnnotation(Guid annotationId, Guid audioId)
    {
        var command = new DeleteAnnotationCommand(annotationId, audioId);
        var result = await deleteAnnotationHandler.Handle(command, default);

        if (result.Value)
        {
            await Clients
                .Group(audioId.ToString())
                .SendAsync("ReceiveAnnotationDelete", annotationId);
            return true;
        }
        return false;
    }
}
