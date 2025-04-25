using KC.Backend.API.Services.Interfaces;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class ClientCommunicator(IHubContext<SignalRHub> hubContext, IDictionary<string, string> connectionsAndGroups) : IClientCommunicator
{
    public IDictionary<string, string> ConnectionsAndGroups { get; } = connectionsAndGroups;
    public Guid BaseGroup { get; } = Guid.Empty;
    public string BaseGroupName { get; } = "lobby";

    public Task MoveToGroupAsync(string connectionId, Guid? group) => group != BaseGroup
        ? MoveToGroupAsync(connectionId, group.ToString())
        : MoveToGroupAsync(connectionId, BaseGroupName);
    public async Task MoveToGroupAsync(string connectionId, string? group)
    {
        if (ConnectionsAndGroups.TryGetValue(connectionId, out var oldGroup))
        {
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, oldGroup);
            ConnectionsAndGroups.Remove(connectionId, out var _);
        }
        
        group ??= string.Empty; // Default to empty string if null (disconnect)
        
        ConnectionsAndGroups.TryAdd(connectionId, group);
        await hubContext.Groups.AddToGroupAsync(connectionId, group);
    }
    
    public Task SendMessageAsync<T>(string connectionId, SignalRMethod<T> method, T message) 
        => !ConnectionsAndGroups.ContainsKey(connectionId) 
            ? throw new Exception($"ConnectionId: {connectionId} does not exist") 
            : hubContext.Clients.Client(connectionId).SendAsync(method.MethodName, message);

    public Task SendMessageToGroupAsync<T>(Guid group, SignalRMethod<T> method, T message) => group != BaseGroup
        ? SendMessageToGroupAsync(group.ToString(), method, message)
        : SendMessageToGroupAsync(BaseGroupName, method, message);

    private Task SendMessageToGroupAsync<T>(string group, SignalRMethod<T> method, T message)
        => hubContext.Clients.Group(group).SendAsync(method.MethodName, message);
}