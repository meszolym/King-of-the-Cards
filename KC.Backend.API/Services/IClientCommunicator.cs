namespace KC.Backend.API.Services;

public interface IClientCommunicator
{
    Dictionary<string, string> ConnectionsAndGroups { get; }
    Task AddToGroupAsync(string connectionId, string group);
    Task RemoveFromGroupAsync(string connectionId, string group);
    Task SendMessageAsync(string connectionId, string method, object? message);
    Task SendMessageToGroupAsync(string group, string method, object message);
}