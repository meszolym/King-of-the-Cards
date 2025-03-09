using System.Net.NetworkInformation;

namespace KC.Backend.Models.GameItems;

public class BettingBox
{
    public int IdxOnTable { get; init; }
    public PhysicalAddress OwnerId { get; set; } = PhysicalAddress.None;
    public List<Hand> Hands { get; private init; } = [new()];
}