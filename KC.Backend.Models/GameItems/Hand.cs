namespace KC.Backend.Models;

public class Hand
{
    public List<Card> Cards { get; set; } = [];
    public bool DealerOwned { get; init; } = false;
    public bool CanBeSplit { get; set; } = true;
    public double Bet { get; set; } = 0;
    public bool Finished { get; set; } = false;
}

public record struct HandValue(int Value, bool IsBlackJack, bool IsPair, bool IsSoft);