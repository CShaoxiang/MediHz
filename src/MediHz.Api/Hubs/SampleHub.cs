using Microsoft.AspNetCore.SignalR;

namespace MediHz.Api.Hubs;

// Example SignalR Hub to reference.
// TODO: Delete once not needed. Make sure to remove the registration in Program.cs as well.
public class SampleHub : Hub
{
    public async Task SendHello()
    {
        await Clients.All.SendAsync("ReceiveHello", "Hello, world!");
    }
}
