using System.Collections.Concurrent;
using KC.Shared.Models.Misc;

namespace KC.Backend.API.Services.Interfaces;

public interface IClientCommunicator
{
    IDictionary<string, string> ConnectionsAndGroups { get; }
    public Guid BaseGroup { get; }
    public string BaseGroupName { get; }
    Task MoveToGroupAsync(string connectionId, Guid? group);
    Task SendMessageAsync<T>(string connectionId, SignalRMethod<T> method, T message);
    Task SendMessageToGroupAsync<T>(Guid group, SignalRMethod<T> method, T message);
}