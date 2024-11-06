using KC.Models.Structs;
using LanguageExt;

namespace KC.Models.Classes;

public class BettingBox(int Id, List<Hand> Hands)
{
    public int Id { get; init; } = Id;
    public List<Hand> Hands { get; init; } = Hands;
    public Option<Player> Owner { get; set; } = Option<Player>.None;
}