using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class SignalRHub(IHubContext<SignalRHub> hubContext) : Hub, IClientCommunicator
{
    public string BaseGroup { get; } = "lobby";
    
    public Dictionary<string, string> ConnectionsAndGroups { get; private init; } = new();
    
    private IHubContext<SignalRHub> _hubContext = hubContext;

    #region Base Hub Methods
    
    public override Task OnConnectedAsync()
    {
        Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        MoveToGroupAsync(Context.ConnectionId, "lobby");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
        if (!ConnectionsAndGroups.TryGetValue(Context.ConnectionId, out var group))
            return base.OnDisconnectedAsync(exception);
        
        MoveToGroupAsync(Context.ConnectionId, null);
        
        return base.OnDisconnectedAsync(exception);
    }
    
    #endregion

    public async Task MoveToGroupAsync(string connectionId, string? group)
    {
        if (ConnectionsAndGroups.TryGetValue(connectionId, out var oldGroup))
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, oldGroup);
            ConnectionsAndGroups.Remove(connectionId);
        }

        if (group is null) return;
        
        ConnectionsAndGroups[connectionId] = group;
        await _hubContext.Groups.AddToGroupAsync(connectionId, group);
    }

    public Task SendMessageAsync(string connectionId, string method, object? message) => _hubContext.Clients.User(connectionId).SendAsync(method, message);

    public Task SendMessageToGroupAsync(string group, string method, object? message) => _hubContext.Clients.Group(group).SendAsync(method, message);
}