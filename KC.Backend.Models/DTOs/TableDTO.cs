using KC.Backend.Models.GameItems;

namespace KC.Backend.Models.DTOs;

public class TableDTO
{
    public IEnumerable<Card> DealerVisibleCards { get; }
    public IEnumerable<BettingBox> BettingBoxes { get; }
}