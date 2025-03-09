using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public class TableDto
{
    public IEnumerable<Card> DealerVisibleCards { get; init; }
    public IEnumerable<BettingBoxDto> BettingBoxes { get; init; }
}