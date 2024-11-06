using System.Collections.Immutable;
using KC.Models.Interfaces;
using LanguageExt;

namespace KC.Models;

public record Hand(Guid TableId, int BoxIdx, int HandIdx, bool Splittable, ImmutableList<Card> Cards, double Bet)
    : IIdentityBearer<(Guid, int, int)>
{
    public (Guid, int, int) Id => (TableId, BoxIdx, HandIdx);

    public void Deconstruct(out Guid TableId, out int BoxIdx, out int HandIdx, out bool Splittable, out ImmutableList<Card> Cards, out double Bet)
    {
        TableId = this.TableId;
        BoxIdx = this.BoxIdx;
        HandIdx = this.HandIdx;
        Splittable = this.Splittable;
        Cards = this.Cards;
        Bet = this.Bet;
    }
}