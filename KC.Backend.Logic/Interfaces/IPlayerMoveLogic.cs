using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerMoveLogic
{
    IEnumerable<Move> GetPossibleMoves(Guid sessionId, int boxIdx, Guid playerId, int handIdx = 0);

    /// <summary>
    /// After making a move, make sure to call GetPossibleMoves and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// </summary>
    void MakeMove(Guid sessionId, int boxIdx, Guid playerId, Move move, int handIdx = 0);

    void UpdateBetOnBox(Guid sessionId, int boxIdx, Guid playerId, double amount, int handIdx = 0);
    void ClaimBettingBox(Guid sessionId, int boxIdx, Guid playerId);
    void DisclaimBettingBox(Guid sessionId, int boxIdx, Guid playerId);
}