using KC.App.Models.Classes;
using Timer = System.Timers.Timer;

namespace KC.App.Logic.Interfaces;

public interface ISessionLogic
{
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet);
    IEnumerable<Session> GetAllSessions();
    bool PurgeOldSessions();
    Session Get(Guid sessionId);
    void Remove(Guid sessionId);
}