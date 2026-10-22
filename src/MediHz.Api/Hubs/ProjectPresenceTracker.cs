namespace MediHz.Api.Hubs;

public sealed record ProjectPresenceSnapshot(Guid ProjectId, long Version, Guid[] OnlineUserIds);

// Ephemeral connection state for this API instance; membership remains in the database.
public sealed class ProjectPresenceTracker
{
    private readonly object gate = new();
    private readonly Dictionary<string, (Guid ProjectId, Guid UserId)> connections = [];
    private long version;

    public ProjectPresenceSnapshot Connect(string connectionId, Guid projectId, Guid userId)
    {
        lock (gate)
        {
            connections[connectionId] = (projectId, userId);
            return Snapshot(projectId);
        }
    }

    public ProjectPresenceSnapshot? Disconnect(string connectionId)
    {
        lock (gate)
        {
            return connections.Remove(connectionId, out var connection)
                ? Snapshot(connection.ProjectId)
                : null;
        }
    }

    private ProjectPresenceSnapshot Snapshot(Guid projectId) =>
        new(projectId, ++version, connections.Values
            .Where(connection => connection.ProjectId == projectId)
            .Select(connection => connection.UserId).Distinct().ToArray());
}
