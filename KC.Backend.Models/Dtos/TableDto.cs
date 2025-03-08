using KC.Backend.Models.GameItems;

namespace KC.Backend.Models.Dtos;

public class TableDto
{
    public IEnumerable<Card> DealerVisibleCards { get; init; }
    public IEnumerable<BettingBoxDto> BettingBoxes { get; init; }
}