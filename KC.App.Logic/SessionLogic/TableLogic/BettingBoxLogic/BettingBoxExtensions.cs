using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Models.Classes;
using KC.App.Models.Structs;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;

public static class BettingBoxExtensions
{
    //unnecesary, can be substituted with indexing.
    //public static Option<Hand> FindHand(this BettingBox box, int Idx) => box.Hands.ElementAtOrDefault(Idx);

    public static Fin<BettingBox> Claim(this BettingBox box, Player player) => box.CheckNoOwner()
    .Map(b =>
    {
        b.Owner = player;
        return b;
    });

    public static Fin<BettingBox> Disclaim(this BettingBox box, Player player) => box.CheckOwner(player)
    .Map(b =>
    {
        b.Owner = Option<Player>.None;
        return b;
    });

    public static Fin<BettingBox> PlaceBet(this BettingBox box, Player player, double amount) => box.CheckOwner(player)
        .Bind(_ => amount >= 0 ? SetBet(box, amount) : FinFail<BettingBox>(Error.New("Bet cannot be less than 0.")));

    private static BettingBox SetBet(this BettingBox box, double amount)
    {
        box.Hands[0].Bet = amount;
        return box;
    }

    public static Fin<BettingBox> CheckOwner(this BettingBox box, Player player) => box.Owner.Match(
        Some: p => p.Id == player.Id ? box : FinFail<BettingBox>(Error.New("Box is not owned by player")),
        None: FinFail<BettingBox>(Error.New("Box has no owner"))
    );

    public static Fin<BettingBox> CheckNoOwner(this BettingBox box) => box.Owner.Match(
        Some: p => FinFail<BettingBox>(Error.New("Box has an owner")),
        None: () => box
    );

    public static Unit ClearHands(this BettingBox box)
    {
        box.Hands.Clear();
        box.Hands.Add(HandService.CreateEmptyHand());
        return unit;
    }

    public static Option<Hand> Split(this BettingBox box, int handIdx, Card card)
    {
        //        case Move.Split:
        //            tupBS.box.Hands.Insert(handIdx+1, new Hand([tupBS.box.Hands[handIdx].Cards[1]],
        //                tupBS.box.Hands[handIdx].Bet, false));

        //            tupBS.box.Hands[handIdx].Cards.RemoveAt(1);
        //            tupBS.box.Hands[handIdx].Cards.Add(tupBS.session.Table.DealingShoe.TakeCard());
        //            tupBS.box.Hands[handIdx].Splittable = false;
        //            break;
        box.Hands.Insert(handIdx + 1, new Hand(new List<Card> { box.Hands[handIdx].Cards[1] }, box.Hands[handIdx].Bet, false));
        box.Hands[handIdx].Cards.RemoveAt(1);
        box.Hands[handIdx].Splittable = false;
        return box.Hands[handIdx].AddCard(card);
    }

}
