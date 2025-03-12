using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Models.GameItems;

public class Player(MacAddress macAddress, string name, double balance, string connectionId)
{
    public MacAddress Id { get; private init; } = macAddress;
    public string ConnectionId { get; set; } = connectionId;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}