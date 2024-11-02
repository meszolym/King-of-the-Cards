using LanguageExt;

namespace KC.Models;

public class BettingBox(Option<Player> owner, List<Hand> hands)
{
    public Option<Player> Owner { get; set; } = owner;
    public List<Hand> Hands { get; set; } = hands;
}