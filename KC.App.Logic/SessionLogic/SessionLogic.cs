using KC.App.Backend.Models.Classes;
using KC.App.Data;
using KC.App.Logic.Interfaces;
using Timer = System.Timers.Timer;


namespace KC.App.Logic.SessionLogic;

public class SessionLogic(IDataStore<Session, Guid> dataStore) : ISessionLogic
{
    /// <summary>
    /// Creates a new session with the given parameters.
    /// Make sure to add a CanBetChangeOnTimerElapsed to the session.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="timerAfterFirstBet"></param>
    /// <returns></returns>
    public Session CreateSession(uint numberOfBoxes, uint numberOfDecks, TickingTimer timerAfterFirstBet)
    {
        var sess = SessionUtilities.CreateEmptySession(numberOfBoxes, numberOfDecks, timerAfterFirstBet)
            .AddCanBetChangeOnTimerElapsed();
        dataStore.Add(sess);
        return sess;
    }

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public bool PurgeOldSessions(TimeSpan oldTimeSpan)
    {
        var purgableSessions = GetAllSessions()
            .Where(s => DateTime.Now - s.LastMoveAt > oldTimeSpan)
            .Select(s => s.Id);

        foreach (var sessionId in purgableSessions)
        {
            dataStore.Remove(sessionId);
        }

        return purgableSessions.Any();
    }

    public Session Get(Guid sessionId) => dataStore.Get(sessionId);
    public void Remove(Guid sessionId) => dataStore.Remove(sessionId);
}