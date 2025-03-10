using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record TableDto(IEnumerable<Card> DealerVisibleCards, IEnumerable<BettingBoxDto> BettingBoxes);