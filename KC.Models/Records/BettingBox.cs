using KC.Models.Classes;

namespace KC.Models.Records;

public record BettingBox(int Id, List<Hand> Hands)
{
    public int Id { get; init; } = Id;
    public List<Hand> Hands { get; init; } = Hands;
}