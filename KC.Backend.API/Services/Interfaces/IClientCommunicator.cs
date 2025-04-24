using System.Collections.Concurrent;

namespace KC.Backend.API.Services.Interfaces;

public interface IClientCommunicator
{
    IDictionary<string, string> ConnectionsAndGroups { get; }
    public string BaseGroup { get; }
    Task MoveToGroupAsync(string connectionId, string group);
    Task SendMessageAsync(string connectionId, string method, object? message);
    Task SendMessageToGroupAsync(string group, string method, object message);
}