using System.Collections.Concurrent;

namespace SignalR.Api.Hubs;

public static class HubStorage
{
    public static ConcurrentDictionary<string, string> ConnectedUsers = new();
}