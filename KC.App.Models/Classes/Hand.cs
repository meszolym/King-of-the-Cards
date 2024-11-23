using KC.App.Models.Structs;
using KC.App.Models.Enums;

namespace KC.App.Models.Classes;

public class Hand(List<Card> cards, double bet, bool splittable)
{
    public List<Card> Cards { get; private set; } = cards;
    public double Bet { get; set; } = bet;
    public bool Splittable { get; set; } = splittable;
    public bool Finished { get; set; } = false;
}