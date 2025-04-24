using KC.Backend.API.Services.Interfaces;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class ClientCommunicator(IHubContext<SignalRHub> hubContext, IDictionary<string, string> connectionsAndGroups) : IClientCommunicator
{
    public IDictionary<string, string> ConnectionsAndGroups { get; } = connectionsAndGroups;
    public string BaseGroup { get; } = "lobby";
    public async Task MoveToGroupAsync(string connectionId, string? group)
    {
        if (ConnectionsAndGroups.TryGetValue(connectionId, out var oldGroup))
        {
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, oldGroup);
            ConnectionsAndGroups.Remove(connectionId, out var _);
        }

        if (group is null) return;
        
        ConnectionsAndGroups.TryAdd(connectionId, group);
        await hubContext.Groups.AddToGroupAsync(connectionId, group);
    }
    
    public Task SendMessageAsync<T>(string connectionId, SignalRMethod<T> method, T message) 
        => !ConnectionsAndGroups.ContainsKey(connectionId) 
            ? throw new Exception($"ConnectionId: {connectionId} does not exist") 
            : hubContext.Clients.Client(connectionId).SendAsync(method.MethodName, message);

    public Task SendMessageToGroupAsync<T>(string group, SignalRMethod<T> method, T message)
        => hubContext.Clients.Group(group).SendAsync(method.MethodName, message);
}