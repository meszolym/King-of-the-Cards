namespace KC.App.Models.Classes;

public class BettingBox(int idx, List<Hand> hands, Player? owner)
{
    public int Idx { get; init; } = idx;
    public List<Hand> Hands { get; init; } = hands;
    public Player? Owner { get; set; } = owner;
}