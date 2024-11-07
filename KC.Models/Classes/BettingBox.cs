using KC.Models.Structs;
using LanguageExt;

namespace KC.Models.Classes;

public class BettingBox(List<Hand> Hands)
{
    public List<Hand> Hands { get; init; } = Hands;
    public Option<Player> Owner { get; set; } = Option<Player>.None;
}