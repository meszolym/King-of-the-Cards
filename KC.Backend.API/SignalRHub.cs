using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API;

public class SignalRHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        this.Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}