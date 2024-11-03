using System.Collections.Immutable;
using KC.Models.Interfaces;
using LanguageExt;

namespace KC.Models;

public record BettingBox(Guid TableId, int BoxIdx, Option<Player> Owner, ImmutableList<Hand> Hands) : IIdentityBearer<(Guid, int)>
{
    public (Guid, int) Id => (TableId, BoxIdx);
    public void Deconstruct(out Guid TableId, out int BoxIdx, out Option<Player> Owner, out ImmutableList<Hand> Hands)
    {
        TableId = this.TableId;
        BoxIdx = this.BoxIdx;
        Owner = this.Owner;
        Hands = this.Hands;
    }
}