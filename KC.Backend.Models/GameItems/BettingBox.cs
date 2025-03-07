namespace KC.Backend.Models;

public class BettingBox
{
    public Player Owner { get; set; } = Player.None;
    public List<Hand> Hands { get; private init; } = [new(), new()];
}