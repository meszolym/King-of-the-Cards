using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using KC.Models.Classes;
using KC.Models.Structs;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.Logic.BettingBoxLogic;

public static class BettingBoxExtensions
{
    //unnecesary, can be substituted with indexing.
    //public static Option<Hand> FindHand(this BettingBox box, int Idx) => box.Hands.ElementAtOrDefault(Idx);

    public static Fin<BettingBox> Claim(this BettingBox box, Player player) => box.Owner.Match(
            Some: er => FinFail<BettingBox>(Error.New("Box already owned by some player")),
            None: () => {
                box.Owner = player;
                return box;
            }
        );

    public static Fin<BettingBox> Unclaim(this BettingBox box, Player player) => box.Owner.Match(
        Some: p => {
            if (p.MacAddress != player.MacAddress)
            {
                return FinFail<BettingBox>(Error.New("Box is not owned by player"));
            }
            box.Owner = Option<Player>.None;
            return box;
        },
        None: () => FinFail<BettingBox>(Error.New("Box has no owner"))
    );

}
