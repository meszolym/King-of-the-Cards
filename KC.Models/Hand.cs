using System.Collections.Immutable;
using LanguageExt;

namespace KC.Models;

public record Hand(bool Splittable, ImmutableList<Card> Cards, double Bet)
{
    public void Deconstruct(out bool Splittable, out ImmutableList<Card> Cards, out double Bet)
    {
        Splittable = this.Splittable;
        Cards = this.Cards;
        Bet = this.Bet;
    }
}