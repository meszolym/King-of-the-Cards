using System.Collections.Concurrent;
using KC.Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class SignalRHub(IHubContext<SignalRHub> hubContext) : Hub, IClientCommunicator
{
    public string BaseGroup { get; } = "lobby";
    
    public ConcurrentDictionary<string, string> ConnectionsAndGroups { get; private init; } = new();

    #region Base Hub Methods
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await MoveToGroupAsync(Context.ConnectionId, BaseGroup);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
        await MoveToGroupAsync(Context.ConnectionId, null);
        await base.OnDisconnectedAsync(exception);
    }
    
    #endregion

    public async Task MoveToGroupAsync(string connectionId, string? group)
    {
        if (ConnectionsAndGroups.TryGetValue(connectionId, out var oldGroup))
        {
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, oldGroup);
            ConnectionsAndGroups.TryRemove(connectionId, out var _);
        }

        if (group is null) return;
        
        ConnectionsAndGroups.TryAdd(connectionId, group);
        await hubContext.Groups.AddToGroupAsync(connectionId, group);
    }

    public Task SendMessageAsync(string connectionId, string method, object? message) 
        => !ConnectionsAndGroups.ContainsKey(connectionId) 
            ? throw new Exception($"ConnectionId: {connectionId} does not exist") 
            : hubContext.Clients.Client(connectionId).SendAsync(method, message);

    public Task SendMessageToGroupAsync(string group, string method, object? message)
        => ConnectionsAndGroups.All(x => x.Value != group)
                ? throw new Exception($"Group: {group} does not exist")
            : hubContext.Clients.Group(group).SendAsync(method, message);
}