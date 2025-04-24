using System.Collections.Generic;
using System.Linq;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;

namespace KC.Backend.Logic.Extensions;

public static class DealerExtensions
{
    public static IEnumerable<Card> GetVisibleCards(this Dealer dealer)
    {
        if (dealer.DealerShowsAllCards)
            return dealer.DealerHand.Cards;
        
        var visibleCards = new List<Card>();
        visibleCards.AddRange(dealer.DealerHand.Cards.Take(1));

        if (visibleCards.Count != 0 && dealer.DealerHand.Cards.Count > 1)
            visibleCards.Add(Card.WithSuitAndFace(Card.CardSuit.None, Card.CardFace.None));
        
        return visibleCards;
    }
}