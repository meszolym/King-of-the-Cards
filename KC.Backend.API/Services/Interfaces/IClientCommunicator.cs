using System.Collections.Concurrent;

namespace KC.Backend.API.Services.Interfaces;

public interface IClientCommunicator
{
    ConcurrentDictionary<string, string> ConnectionsAndGroups { get; }
    Task MoveToGroupAsync(string connectionId, string group);
    Task SendMessageAsync(string connectionId, string method, object? message);
    Task SendMessageToGroupAsync(string group, string method, object message);
}