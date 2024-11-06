using System.Collections.Immutable;
using KC.Models.Classes;

namespace KC.Models.Records;

public record Table(ImmutableList<BettingBox> Boxes, Shoe DealingShoe)
{
    public ImmutableList<BettingBox> Boxes { get; init; } = Boxes;
    public Shoe DealingShoe { get; init; } = DealingShoe;
}