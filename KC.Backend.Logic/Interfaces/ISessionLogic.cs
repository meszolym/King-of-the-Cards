using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Interfaces;

public interface ISessionLogic
{
    /// <summary>
    /// Creates an empty session. Make sure to subscribe to events of the timer.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="shuffleCardPlacement"></param>
    /// <param name="shuffleCardRange"></param>
    /// <param name="bettingTimerSeconds"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, int shuffleCardRange, int bettingTimerSeconds, Random? random = null);

    Session Get(Guid sessionId);
    IEnumerable<Session> GetAll();
    bool PurgeOldSessions(TimeSpan oldTimeSpan);
    void UpdateTimer(Guid sessionId);
    void TransferTurn(Guid sessionId);
    void FinishAllHandsInPlay(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    void PayOutBets(Guid sessionId);

    void ClearHands(Guid sessionId);
}