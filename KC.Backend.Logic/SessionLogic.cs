using KC.Backend.Logic.GameItemsLogic;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

public class SessionLogic(IList<Session> dataStore) : ISessionLogic
{
    /// <summary>
    /// Creates a new session with the given parameters.
    /// Make sure to add a CanBetChangeOnTimerElapsed to the session.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <returns></returns>
    public Session CreateSession(uint numberOfBoxes, uint numberOfDecks, TickingTimer bettingTimer)
    {
        var shoe = CardShoeUtilities.CreateUnshuffledShoe(numberOfDecks);
        var table = new Table((int)numberOfBoxes, shoe);
        
        var sess = new Session(table, bettingTimer);
        
        dataStore.Add(sess);
        return sess;
    }

    public IEnumerable<Session> GetAllSessions() => dataStore;

    public bool PurgeOldSessions(TimeSpan oldTimeSpan)
    {
        var purgableSessionIds = GetAllSessions()
            .Where(s => DateTime.Now - s.LastMoveMadeAt > oldTimeSpan)
            .Select(s => s.Id);

        if (!purgableSessionIds.Any())
        {
            return false;
        }
        
        foreach (var sessionId in purgableSessionIds)
        {
            Remove(sessionId);
        }

        return true;
    }

    public Session Get(Guid sessionId) => dataStore.Single(s => s.Id == sessionId);
    public void Remove(Guid sessionId) => dataStore.Remove(Get(sessionId));
}