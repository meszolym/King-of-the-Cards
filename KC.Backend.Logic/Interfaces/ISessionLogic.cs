using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Interfaces;

public interface ISessionLogic
{
    /// <summary>
    /// Creates a new session with the given parameters.
    /// Make sure to add a CanBetChangeOnTimerElapsed to the session.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <returns></returns>
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, TickingTimer bettingTimer);

    IEnumerable<Session> GetAllSessions();
    bool PurgeOldSessions(TimeSpan oldTimeSpan);
    Session Get(Guid sessionId);
    void Remove(Guid sessionId);
    void UpdateTimer(Guid sessionId);
}