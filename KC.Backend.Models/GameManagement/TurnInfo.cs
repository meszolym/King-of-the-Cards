namespace KC.Backend.Models.GameManagement;

public record struct TurnInfo(bool PlayersTurn, int BoxIdx, int HandIdx);