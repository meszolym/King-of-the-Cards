using System.Collections.Concurrent;
using KC.Backend.API.Services.Interfaces;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Services;

public class SignalRHub(IClientCommunicator clientCommunicator) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await clientCommunicator.MoveToGroupAsync(Context.ConnectionId, clientCommunicator.BaseGroup);
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
        await clientCommunicator.MoveToGroupAsync(Context.ConnectionId, null);
        await base.OnDisconnectedAsync(exception);
    }
}