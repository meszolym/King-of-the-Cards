using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Models.GameItems;

public class Player
{
    public Player(MacAddress macAddress, string name, double balance, string connectionId)
    {
        Id = macAddress;
        Name = name;
        Balance = balance;
        ConnectionId = connectionId;
    }
    
    public Player() { }
    
    public MacAddress Id { get; private init; }
    public string ConnectionId { get; set; } 
    public string Name { get; set; }
    public double Balance { get; set; }
}