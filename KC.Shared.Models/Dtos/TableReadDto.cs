using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public record TableReadDto(IEnumerable<Card> DealerVisibleCards, IEnumerable<BettingBoxReadDto> BettingBoxes);