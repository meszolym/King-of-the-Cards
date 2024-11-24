using System.Collections.Immutable;
using KC.App.Logic.CardLogic;
using KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.App.Models.Classes.Hand;
using KC.App.Models.Enums;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;

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

    public static string GetValueString(this DealerHand hand)
    {
        if (hand.GetValue().IsBlackJack) return "BJ";
        if (hand.GetValue().IsSoft && !hand.Finished) return $"S{hand.GetValue().Value}";
        return hand.GetValue().Value.ToString();
    }

    public static string GetValueString(this PlayerHand hand)
    {
        if (hand.GetValue().IsBlackJack) return "BJ";
        if (!hand.Finished)
        {
            if (hand.GetValue().IsSoft && hand.GetValue().IsPair && hand.Splittable) return "P11"; //Pair of Aces
            if (hand.GetValue().IsSoft) return $"S{hand.GetValue().Value}";
            if (hand.GetValue().IsPair && hand.Splittable) return $"P{hand.GetValue().Value / 2}";
        }
        return hand.GetValue().Value.ToString();
    }

    public static List<Move> GetPossibleActions(this PlayerHand hand)
    {
        if (hand.Cards.Count < 2) throw new InvalidOperationException("Cannot get actions for incomplete hand");
        if (hand.Finished) return []; //no action on finished hands
        if (hand.GetValue().Value >= 21) return []; //no action on bust hands
        if (hand.Cards[0].Face == CardFace.Ace && !hand.Splittable) return []; //no action on split aces (they automatically get only one card)

        var moves = new List<Move>
        {
            Move.Stand, //can always stand
            Move.Hit //can hit on any card
        };

        if (hand.Cards.Count() == 2) moves.Add(Move.Double); //can double on any two cards
        if (hand.Splittable && hand.GetValue().IsPair) moves.Add(Move.Split); //can split if the hand has not been split (splittable) and it is a pair
        return moves;
    }
}
