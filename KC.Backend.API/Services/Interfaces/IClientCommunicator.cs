using System.Collections.Concurrent;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services.Interfaces;

public interface IClientCommunicator
{
    IDictionary<string, string> ConnectionsAndGroups { get; }
    public string BaseGroup { get; }
    Task MoveToGroupAsync(string connectionId, string? group);
    Task SendMessageAsync<T>(string connectionId, SignalRMethod<T> method, T message);
    Task SendMessageToGroupAsync<T>(string group, SignalRMethod<T> method, T message);
}