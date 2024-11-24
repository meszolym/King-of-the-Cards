using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection.Metadata.Ecma335;
using KC.App.Data;
using KC.App.Logic.Interfaces;
using KC.App.Logic.SessionLogic.TableLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.App.Models.Classes;
using KC.App.Models.Enums;
using Timer = System.Timers.Timer;


namespace KC.App.Logic.SessionLogic;

public class SessionLogic(IDataStore<Session, Guid> dataStore) : ISessionLogic
{
    public Session CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet)
    {
        var sess = SessionService.CreateEmptySession(numberOfBoxes, numberOfDecks, timerAfterFirstBet)
            .AddCanBetChangeOnTimerElapsed();
        dataStore.Add(sess);
        return sess;
    }

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public bool PurgeOldSessions()
    {
        var purgableSessions = GetAllSessions()
            .Where(s => DateTime.Now - s.LastMoveAt > TimeSpan.FromMinutes(10))
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