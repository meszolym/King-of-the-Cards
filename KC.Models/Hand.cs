using LanguageExt;

namespace KC.Models;

public class Hand(BettingBox parentBox, bool splittable, List<Card> cards, double bet)
{
    public BettingBox ParentBox { get; set; } = parentBox;
    public bool Splittable { get; set; } = splittable;
    public List<Card> Cards { get; set; } = cards;
    public double Bet { get; set; } = bet;
}