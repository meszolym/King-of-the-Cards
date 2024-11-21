using System.Collections.Immutable;
using KC.App.Models.Classes;
using LanguageExt;

namespace KC.App.Models.Records;

public record Table(ImmutableList<BettingBox> Boxes, Shoe DealingShoe);