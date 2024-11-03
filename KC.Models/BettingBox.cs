using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Models.Interfaces;
using LanguageExt;

namespace KC.Models;

public record BettingBox(Guid TableId, int BoxIdx, Option<string> OwnerId, ImmutableList<Hand> Hands) : IIdentityBearer<(Guid, int)>
{
    public (Guid, int) Id => (TableId, BoxIdx);

    [NotMapped] 
    public Option<string> OwnerId = OwnerId;

    public string? OwnerIdNullable => OwnerId.Match(s => s, () => null);

    public void Deconstruct(out Guid TableId, out int BoxIdx, out Option<string> OwnerId, out ImmutableList<Hand> Hands)
    {
        TableId = this.TableId;
        BoxIdx = this.BoxIdx;
        OwnerId = this.OwnerId;
        Hands = this.Hands;
    }


    [NotMapped]
    public virtual Table Table { get; }

    [NotMapped]
    public virtual Player? OwnerNullable { get; }

    [NotMapped]
    public virtual Option<Player> Owner => OwnerNullable.ToOption();
}