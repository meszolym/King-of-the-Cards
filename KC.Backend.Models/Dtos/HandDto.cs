using KC.Backend.Models.GameItems;

namespace KC.Backend.Models.Dtos;

public class HandDto
{
    public IEnumerable<Card> Cards { get; init; }
    public double Bet { get; init; }
}