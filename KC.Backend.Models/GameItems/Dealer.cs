using KC.Shared.Models.GameItems;

namespace KC.Backend.Models.GameItems;

public class Dealer
{
    public Hand DealerHand { get; set; } = new(){DealerOwned = true};
    public bool DealerShowsAllCards { get; set; } = false;
    public IEnumerable<Card> DealerVisibleCards => DealerShowsAllCards ? DealerHand.Cards : DealerHand.Cards.Take(1);
}