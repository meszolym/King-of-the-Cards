using System.Collections.Immutable;
using LanguageExt;

namespace KC.Models;

public record BettingBox(Option<Player> Owner, ImmutableList<Hand> Hands)
{
    public Option<Player> Owner { get; } = Owner;
    public ImmutableList<Hand> Hands { get; } = Hands;
}