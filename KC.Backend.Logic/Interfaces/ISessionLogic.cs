using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Interfaces;

public interface ISessionLogic
{
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, TickingTimer bettingTimer);
    IEnumerable<Session> GetAllSessions();
    bool PurgeOldSessions(TimeSpan oldTimeSpan);
    Session Get(Guid sessionId);
    void Remove(Guid sessionId);
}