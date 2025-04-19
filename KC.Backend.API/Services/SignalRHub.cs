using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class SignalRHub(IHubContext<SignalRHub> hubContext) : Hub, IClientCommunicator
{
    public Dictionary<string, string> ConnectionsAndGroups { get; private init; } = new();
    
    private IHubContext<SignalRHub> _hubContext = hubContext;

    #region Base Hub Methods
    
    public override Task OnConnectedAsync()
    {
        Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        AddToGroupAsync(Context.ConnectionId, "lobby");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
        if (!ConnectionsAndGroups.TryGetValue(Context.ConnectionId, out var group))
            return base.OnDisconnectedAsync(exception);
        
        RemoveFromGroupAsync(Context.ConnectionId, group);
        return base.OnDisconnectedAsync(exception);
    }
    
    #endregion

    public Task AddToGroupAsync(string connectionId, string group)
    {
        ConnectionsAndGroups[connectionId] = group;
        return _hubContext.Groups.AddToGroupAsync(connectionId, group);
    }
    
    public Task RemoveFromGroupAsync(string connectionId, string group)
    {
        ConnectionsAndGroups.Remove(connectionId);
        return _hubContext.Groups.RemoveFromGroupAsync(connectionId, group);
    }

    public Task SendMessageAsync(string connectionId, string method, object? message) => _hubContext.Clients.User(connectionId).SendAsync(method, message);

    public Task SendMessageToGroupAsync(string group, string method, object? message) => _hubContext.Clients.Group(group).SendAsync(method, message);
}