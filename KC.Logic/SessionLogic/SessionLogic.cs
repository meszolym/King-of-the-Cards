using System.Reactive.Subjects;
using KC.Data;
using KC.Logic.SessionLogic.TableLogic;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Models.Classes;
using KC.Models.Structs;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Timer = System.Timers.Timer;
using Unit = LanguageExt.Unit;

namespace KC.Logic.SessionLogic;

internal class SessionLogic(IDataStore<Session, Guid> dataStore)
{
    private readonly Subject<TurnInfo> _turnChangedSubject = new();
    public IObservable<TurnInfo> TurnChangedObservable => _turnChangedSubject;

    public Fin<Unit> CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) =>
        dataStore.Add(SessionService.CreateEmptySession(numberOfBoxes, numberOfDecks, timerAfterFirstBet));

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public Fin<BettingBox> ClaimBox(Guid sessionId, int boxIdx, Player player) => dataStore
        .Get(sessionId).Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not claim boxes at this time."))).Bind(b =>b.Claim(player));

    public Fin<BettingBox> DisclaimBox(Guid sessionId, int boxIdx, Player player) => dataStore
        .Get(sessionId).Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not disclaim boxes at this time."))).Bind(b => b.Disclaim(player));

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// </summary>
    public Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount) => dataStore
        .Get(sessionId).Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not place bets at this time."))).Bind(b => b.PlaceBet(player, amount));

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Fin of bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public Fin<bool> UpdateTimer(Guid sessionId) => dataStore.Get(sessionId).Map(s =>
    {
        if (s.Table.Boxes.Any(b => b.Hands[0].Bet > 0) && !s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Start();
        else if(s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Stop();

        return s.BetPlacementTimer.Enabled;
    });

    public Fin<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId) =>
        dataStore.Get(sessionId).Map(s => (s.CurrentBoxIdx, s.CurrentHandIdx));

    //startdealing

    //getmoves

    //makemove -> if no more moves, transfer turn to next player

    //transferturn -> if everyone has played, fire and forget endround

    //endround (if everyone has played)

    //resetsession
}