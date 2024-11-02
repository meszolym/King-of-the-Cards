using System.Collections.Immutable;
using LanguageExt;

namespace KC.Models;

public record Hand(bool Splittable, ImmutableList<Card> Cards, double Bet)
{
    public bool Splittable { get; } = Splittable;
    public ImmutableList<Card> Cards { get; } = Cards;
    public double Bet { get; } = Bet;
}