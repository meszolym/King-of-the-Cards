using KC.Backend.Models;

namespace KC.Backend.Logic.GameItemsLogic;

public static class HandUtilities
{

    /// <summary>
    /// Gets the value of the hand. Does not take hand.Splittable into account.
    /// </summary>
    /// <param name="hand"></param>
    /// <returns>HandValue instance with the necessary information.</returns>
    public static HandValue GetValue(this Hand hand)
    {
        bool containsAce = hand.Cards.Any(x => x.Face == Card.CardFace.Ace);
        int value = hand.Cards.Sum(x => x.GetValue());
        bool isSoft = containsAce && value <= 11;

        //ace was counted as one in the card evaluation, so we add 10 to the value if possible.
        if (isSoft) value += 10;

        bool isBlackJack = hand.Cards.Count() == 2 && containsAce && value == 21;
        bool isPair = hand.Cards.Count == 2 && hand.Cards[0].Face == hand.Cards[1].Face;

        return new HandValue(value, isBlackJack, isPair, isSoft);
    }

    public static string GetHandValueString(this Hand hand)
    {
        if (hand.GetValue().IsBlackJack) return "BJ";
        
        if (!hand.Finished)
        {
            if (!hand.DealerOwned)
            {
                if (hand.GetValue().IsSoft && hand.GetValue().IsPair && hand.CanBeSplit) return "P11"; //Pair of Aces
            }
            if (hand.GetValue().IsSoft && !hand.Finished) return $"S{hand.GetValue().Value}";
            if (!hand.DealerOwned)
            {
                if (hand.GetValue().IsPair && hand.CanBeSplit) return $"P{hand.GetValue().Value / 2}";
            }
        }
        
        return hand.GetValue().Value.ToString();
        
    }
}
