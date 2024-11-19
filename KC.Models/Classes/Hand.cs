using KC.Models.Enums;
using KC.Models.Structs;
using LanguageExt;

namespace KC.Models.Classes;

public class Hand(List<Card> cards, double bet, bool splittable)
{
    public List<Card> Cards { get; private set; } = cards;
    public double Bet { get; set; } = bet;
    public bool Splittable { get; set; } = splittable;
}