using KC.Shared.Models.GameItems;

namespace KC.Shared.Models.Dtos;

public class HandDto
{
    public IEnumerable<Card> Cards { get; init; }
    public double Bet { get; init; }
}