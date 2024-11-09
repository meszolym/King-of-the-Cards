using KC.Models.Structs;
using LanguageExt;

namespace KC.Models.Classes;

public class BettingBox(List<Hand> hands, Option<Player> owner)
{
    public List<Hand> Hands { get; init; } = hands;
    public Option<Player> Owner { get; set; } = owner;
}