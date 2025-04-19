using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class SignalRHub(IHubContext<SignalRHub> hubContext) : Hub, IClientCommunicator
{
    public string BaseGroup { get; } = "lobby";
    
    public Dictionary<string, string> ConnectionsAndGroups { get; private init; } = new();
    
    private IHubContext<SignalRHub> _hubContext = hubContext;

    #region Base Hub Methods
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await MoveToGroupAsync(Context.ConnectionId, "lobby");
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