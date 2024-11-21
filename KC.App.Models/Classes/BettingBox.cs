using LanguageExt;

namespace KC.App.Models.Classes;

public class BettingBox(List<Hand> hands, Option<Player> owner)
{
    public List<Hand> Hands { get; init; } = hands;
    public Option<Player> Owner { get; set; } = owner;
}