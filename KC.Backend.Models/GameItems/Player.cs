using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Models.GameItems;

public class Player
{
    public Player(string name, double balance, string connectionId)
    {
        Name = name;
        Balance = balance;
        ConnectionId = connectionId;
    }
    
    public Player() { }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ConnectionId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public double Balance { get; set; }
}