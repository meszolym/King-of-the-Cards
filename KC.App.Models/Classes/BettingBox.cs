using KC.App.Backend.Models.Classes.Hand;

namespace KC.App.Backend.Models.Classes;

public class BettingBox(int idx, List<PlayerHand> hands, Player? owner)
{
    public int Idx { get; init; } = idx;
    public List<PlayerHand> Hands { get; init; } = hands;
    public Player? Owner { get; set; } = owner;
}