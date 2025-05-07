using KC.Shared.Models.GameItems;

namespace KC.Backend.Models.GameItems;

public class Dealer
{
    public Hand Hand { get; set; } = new(){DealerOwned = true};
    public bool ShowAllCards { get; set; } = false;
}