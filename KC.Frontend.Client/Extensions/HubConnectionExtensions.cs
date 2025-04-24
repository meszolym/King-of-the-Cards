using System;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.SignalR.Client;

namespace KC.Frontend.Client.Extensions;

public static class HubConnectionExtensions
{
    public static IDisposable On<T>(this HubConnection hubConnection, SignalRMethod<T> method, Action<T> handler) => hubConnection.On<T>(method.MethodName, handler);
}