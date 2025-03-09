namespace KC.Shared.Models.GameManagement;

public record struct TurnInfo(bool PlayersTurn, int BoxIdx, int HandIdx);