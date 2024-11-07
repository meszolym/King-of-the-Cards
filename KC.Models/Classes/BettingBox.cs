using KC.Models.Structs;
using LanguageExt;

namespace KC.Models.Classes;

public class BettingBox
{
    public List<Hand> Hands { get; init; }
    public Option<Player> Owner { get; set; }

    /// <summary>
    /// Creates an empty betting box that has no owner and one empty hand.
    /// </summary>
    public BettingBox()
    {
        Hands = [new Hand([],0,true)];
        Owner = Option<Player>.None;
    }

}