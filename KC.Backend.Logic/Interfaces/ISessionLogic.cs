using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Interfaces;

public interface ISessionLogic
{
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, int shuffleCardRange, TickingTimer bettingTimer, Random? random = null);
    bool PurgeOldSessions(TimeSpan oldTimeSpan);
    void UpdateTimer(Guid sessionId);
    void TransferTurn(Guid sessionId);
    void FinishAllHandsInPlay(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// </summary>
    void PayOutBets(Guid sessionId);

    void ClearHands(Guid sessionId);
}