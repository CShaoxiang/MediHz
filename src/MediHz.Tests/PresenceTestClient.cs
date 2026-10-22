using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace MediHz.Tests;

// Minimal SignalR JSON client, to test the real WebSocket transport without another dependency.
internal sealed class PresenceTestClient : IDisposable
{
    private readonly ClientWebSocket socket = new();
    private string pending = "";

    public async Task ConnectAsync(Uri uri, string token)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        socket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
        await socket.ConnectAsync(uri, timeout.Token);
        await socket.SendAsync(
            Encoding.UTF8.GetBytes("{\"protocol\":\"json\",\"version\":1}\u001e").AsMemory(),
            WebSocketMessageType.Text, true, timeout.Token);
    }

    public async Task ExpectOnlineAsync(params Guid[] expectedUsers)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (true)
        {
            var separator = pending.IndexOf('\u001e');
            if (separator < 0)
            {
                var buffer = new byte[4096];
                var received = await socket.ReceiveAsync(buffer.AsMemory(), timeout.Token);
                Assert.NotEqual(WebSocketMessageType.Close, received.MessageType);
                pending += Encoding.UTF8.GetString(buffer, 0, received.Count);
                continue;
            }
            var record = pending[..separator];
            pending = pending[(separator + 1)..];
            using var json = JsonDocument.Parse(record);
            var message = json.RootElement;
            if (!message.TryGetProperty("target", out var target)
                || target.GetString() != "PresenceChanged")
                continue;
            var users = message.GetProperty("arguments")[0].GetProperty("onlineUserIds")
                .EnumerateArray().Select(u => u.GetGuid()).ToHashSet();
            if (users.SetEquals(expectedUsers))
                return;
        }
    }

    public void Dispose() => socket.Dispose();
}
