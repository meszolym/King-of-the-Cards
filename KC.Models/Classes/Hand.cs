using KC.Models.Structs;

namespace KC.Models.Classes;

public class Hand(List<Card> cards, double bet)
{
    public List<Card> Cards { get; private set; } = cards;
    public double Bet { get; set; } = bet;
}