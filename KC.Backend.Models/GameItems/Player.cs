using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Models.GameItems;

public class Player
{
    public Player(MacAddress macAddress, string name, double balance, string connectionId)
    {
        Mac = macAddress;
        Name = name;
        Balance = balance;
        ConnectionId = connectionId;
    }
    
    public Player() { }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public MacAddress Mac { get; private init; }
    public string ConnectionId { get; set; } 
    public string Name { get; set; }
    public double Balance { get; set; }
}