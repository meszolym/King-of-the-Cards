using System.Collections.Immutable;
using LanguageExt;

namespace KC.Models;

public record BettingBox(Option<Player> Owner, ImmutableList<Hand> Hands)
{
    public void Deconstruct(out Option<Player> Owner, out ImmutableList<Hand> Hands)
    {
        Owner = this.Owner;
        Hands = this.Hands;
    }
}