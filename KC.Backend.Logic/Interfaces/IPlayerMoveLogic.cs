using KC.Backend.Models.GameItems;

namespace KC.Backend.Logic.Interfaces;

public interface IPlayerMoveLogic
{

    /// <summary>
    /// After making a move, make sure to call GetPossibleMoves and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// </summary>
    void MakeMove(Guid sessionId, int boxIdx, Guid playerId, Move move, int handIdx = 0);
}