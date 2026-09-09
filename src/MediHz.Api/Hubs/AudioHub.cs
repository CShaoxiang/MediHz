using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

public class AudioHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task SendRegionData(string audioId, string regionId)
    {
        await Clients.All.SendAsync("ReceiveRegionData", audioId, regionId);
    }
}
