using System.Net.NetworkInformation;

namespace KC.Backend.Models.GameItems;

public class Player(PhysicalAddress macAddress, string name, double balance, string connectionId)
{
    public PhysicalAddress Id { get; private init; } = macAddress;
    public string ConnectionId { get; set; } = connectionId;
    public string Name { get; set; } = name;
    public double Balance { get; set; } = balance;
}