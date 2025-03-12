using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Models.GameItems;

public class BettingBox
{
    public int IdxOnTable { get; init; }
    public MacAddress OwnerId { get; set; } = MacAddress.None;
    public List<Hand> Hands { get; private init; } = [new()];
}