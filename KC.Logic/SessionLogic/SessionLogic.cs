using KC.Data;
using KC.Logic.SessionLogic.TableLogic;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Models.Classes;
using LanguageExt;
using System.Reactive;
using System.Reactive.Linq;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Unit = LanguageExt.Unit;

namespace KC.Logic.SessionLogic
{
    internal class SessionLogic(IDataStore<Session, Guid> dataStore)
    {
        public Fin<Unit> CreateSession(uint numberOfBoxes, uint numberOfDecks, IObservable<long> timer) =>
            dataStore.Add(SessionService.CreateEmptySession(numberOfBoxes, numberOfDecks));

        public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();


        public Fin<BettingBox> ClaimBox(Guid sessionId, int boxIdx, Player player) =>
            dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)).Bind(b =>b.Claim(player));

        public Fin<BettingBox> UnclaimBox(Guid sessionId, int boxIdx, Player player) =>
            dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)).Bind(b => b.Unclaim(player));

        public Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount) => dataStore
            .Get(sessionId).Bind(s => 
                s.CanPlaceBets 
                    ? s.Table.GetBettingBox(boxIdx) 
                    : FinFail<BettingBox>(Error.New("Bets may not be placed at this time")))
            .Bind(b => b.PlaceBet(player, amount));

        //public Fin<Unit> InteractWithTimer(Guid sessionId, Action<Session> bettingStoppedHandler, Action<Session> bettingTimerStartedHandler, Action<Session> bettingTimerStopperHandler) => dataStore.Get(sessionId)
        //    .Bind(s => s.Table.Boxes.Any(b => b.Hands[0].Bet > 0) ? StartBetTimer(sessionId, bettingStoppedHandler, bettingTimerStartedHandler) : StopBetTimer(sessionId));

        //private Fin<IObservable<long>> StartBetTimer(Guid sessionId) => dataStore.Get(sessionId).Map(s =>
        //{
        //    //resetting and starting timer
        //    s.Timer = Observable.Timer(TimeSpan.FromSeconds());
        //    s.TimerSubscription = s.Timer.Subscribe<long>(l =>
        //    {
        //        s.CanPlaceBets = false;
        //    });
        //    return s.Timer;
        //});

        //private Fin<Unit> StopBetTimer(Guid sessionId) => dataStore.Get(sessionId).Map(s =>
        //{
        //    s.TimerSubscription.Dispose();
        //    return Unit.Default;
        //});

        public Fin<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId) =>
            dataStore.Get(sessionId).Map(s => (s.CurrentBoxIdx, s.CurrentHandIdx));
    }
}
