namespace KC.Backend.Models.GameItems;

public class Hand
{
    public List<Card> Cards { get; set; } = [];
    public bool DealerOwned { get; init; } = false;
    public bool FromSplit { get; set; } = false;
    public double Bet { get; set; } = 0;
    public bool Finished { get; set; } = false;
}

public record struct HandValue(int NumberValue, bool IsBlackJack, bool IsPair, bool IsSoft);