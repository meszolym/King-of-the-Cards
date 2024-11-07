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

    public static Fin<BettingBox> Claim(this BettingBox box, Player player) => box.CheckNoOwner()
    .Map(b => {
        b.Owner = player;
        return b;
    });

    public static Fin<BettingBox> Unclaim(this BettingBox box, Player player) => box.CheckOwner(player)
    .Map(b => {
        b.Owner = Option<Player>.None;
        return b;
    });

    public static Fin<BettingBox> PlaceBet(this BettingBox box, Player player, int amount) => box.CheckOwner(player)
    .Bind(b => amount >= 0 ? box.SetBet(amount) : FinFail<BettingBox>(Error.New("Bet cannot be less than 0")));

    private static BettingBox SetBet(this BettingBox box, int amount)
    {
        box.Hands[0].Bet = amount;
        return box;
    }

    public static Fin<BettingBox> CheckOwner(this BettingBox box, Player player) => box.Owner.Match<Fin<BettingBox>>(
        Some: p => p.MacAddress == player.MacAddress ? box : FinFail<BettingBox>(Error.New("Box is not owned by player")),
        None: FinFail<BettingBox>(Error.New("Box has no owner"))
    );

    public static Fin<BettingBox> CheckNoOwner(this BettingBox box) => box.Owner.Match<Fin<BettingBox>>(
        Some: p => FinFail<BettingBox>(Error.New("Box has an owner")),
        None: () => box
    );

}
