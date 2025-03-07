namespace KC.Backend.Models.GameItems;

public class BettingBox
{
    public Guid OwnerId { get; set; } = Guid.Empty;
    public List<Hand> Hands { get; private init; } = [new(), new()];
}