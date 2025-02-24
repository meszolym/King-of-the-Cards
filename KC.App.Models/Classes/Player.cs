using KC.App.Backend.Models.Interfaces;

namespace KC.App.Backend.Models.Classes;

public class Player(string id, string name, double balance, string connectionId) : IIdentityBearer<string>
{
    public string Id { get; init; } = id;
    public string ConnectionId { get; set; } = connectionId;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}