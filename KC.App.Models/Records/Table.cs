using System.Collections.Immutable;
using KC.App.Models.Classes;
using KC.App.Models.Classes.Hand;
using Newtonsoft.Json;

namespace KC.App.Models.Records;

public record Table(ImmutableList<BettingBox> Boxes, [property: JsonIgnore] Shoe DealingShoe, Hand DealerHand);