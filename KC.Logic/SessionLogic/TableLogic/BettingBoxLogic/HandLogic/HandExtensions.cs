using System.Collections.Immutable;
using KC.Logic.CardLogic;
using KC.Logic.Other_extensions;
using KC.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Enums;
using KC.Models.Structs;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace KC.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;

public static class HandExtensions
{

    /// <summary>
    /// Gets the value of the hand. Does not take hand.Splittable into account.
    /// </summary>
    /// <param name="hand"></param>
    /// <returns>HandValue instance with the necessary information.</returns>
    public static HandValue GetValue(this Hand hand)
    {
        bool containsAce = hand.Cards.Any(x => x.Face == CardFace.Ace);
        int value = hand.Cards.Sum(x => x.GetValue());
        bool isSoft = containsAce && value <= 11;

        //ace was counted as one in the card evaluation, so we add 10 to the value if possible.
        if (isSoft) value += 10;

        bool isBlackJack = hand.Cards.Count() == 2 && containsAce && value == 21;
        bool isPair = hand.Cards.Count == 2 && hand.Cards[0].Face == hand.Cards[1].Face;

        return new HandValue(value, isBlackJack, isPair, isSoft);
    }


    public static string GetValueString(this Hand hand) => (hand, val: hand.GetValue()) switch
    {
        { val.IsBlackJack: true } => "BJ",
        { val.IsSoft: true, val.IsPair: true, hand.Splittable: true } => "P11", //Pair of Aces
        { val.IsSoft: true } => $"S{hand.GetValue().Value}",
        { val.IsPair: true, hand.Splittable: true } => $"P{hand.GetValue().Value / 2}",
        _ => hand.GetValue().Value.ToString()
    };

    public static Fin<Seq<Move>> GetPossibleActions(this Hand hand)
    => (hand, val: hand.GetValue(), firstCard: hand.Cards.ElementAtOrDefault(0)) switch
    {
        { hand.Cards.Count: < 2 } => FinFail<Seq<Move>>(Error.New("Can't get actions on incomplete hand")),
        { val.Value: >= 21 } => new Seq<Move>(), //no action on bust hands
        { firstCard.Face: CardFace.Ace, hand.Splittable: false } => new Seq<Move>(), //no action on split aces (they automatically get only one card)
        _ => new Seq<Move>()
            .Add(Move.Hit) //can hit on any card
            .AddIf(hand.Cards.Count() == 2, Move.Double) //can double on any two cards
            .AddIf(hand.Splittable && hand.GetValue().IsPair, Move.Split) //can split if the hand has not been split (splittable) and it is a pair
    };
}
