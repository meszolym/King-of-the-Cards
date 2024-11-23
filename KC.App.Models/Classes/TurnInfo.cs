namespace KC.App.Models.Classes;

public class TurnInfo(bool playersTurn, int boxIdx, int handIdx)
{
    public bool PlayersTurn { get; set; } = playersTurn;
    public int BoxIdx { get; set; } = boxIdx;
    public int HandIdx { get; set; } = handIdx;
}