using System.Collections.Immutable;
using KC.Models.Classes;
using LanguageExt;

namespace KC.Models.Records;

public record Table(ImmutableList<BettingBox> Boxes, Shoe DealingShoe);