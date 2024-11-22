using LanguageExt;

namespace KC.App.Models.Classes;

public class BettingBox(int idx, List<Hand> hands, Option<Player> owner)
{
    public int Idx { get; init; } = idx;
    public List<Hand> Hands { get; init; } = hands;
    public Option<Player> Owner { get; set; } = owner;
}