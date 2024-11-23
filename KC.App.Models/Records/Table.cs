using System.Collections.Immutable;
using KC.App.Models.Classes;

namespace KC.App.Models.Records;

public record Table(ImmutableList<BettingBox> Boxes, Shoe DealingShoe, Hand DealerHand);