using System.Collections.Generic;
using System.Linq;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;

namespace KC.Backend.Logic.Extensions;

public static class DealerExtensions
{
    public static IEnumerable<Card> GetVisibleCards(this Dealer dealer)
    {
        if (dealer.ShowAllCards)
            return dealer.Hand.Cards;
        
        var visibleCards = new List<Card>();
        visibleCards.AddRange(dealer.Hand.Cards.Take(1));

        if (visibleCards.Count != 0 && dealer.Hand.Cards.Count > 1)
            visibleCards.Add(Card.WithSuitAndFace(Card.CardSuit.None, Card.CardFace.None));
        
        return visibleCards;
    }
}