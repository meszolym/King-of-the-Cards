using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public class TableReadDto
{
    public IEnumerable<Card> DealerVisibleCards { get; set; }
    public IEnumerable<BettingBoxReadDto> BettingBoxes { get; set; }
}