namespace KC.Backend.Models.GameItems;

public class BettingBox
{
    public int IdxOnTable { get; init; }
    public Guid OwnerId { get; set; } = Guid.Empty;
    public List<Hand> Hands { get; private init; } = [new()];
}