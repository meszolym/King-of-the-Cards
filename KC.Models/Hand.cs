using LanguageExt;

namespace KC.Models;

public class Hand(bool splittable, List<Card> cards, double bet)
{
    public bool Splittable { get; set; } = splittable;
    public List<Card> Cards { get; set; } = cards;
    public double Bet { get; set; } = bet;
}