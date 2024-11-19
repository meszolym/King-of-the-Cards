﻿using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using KC.Data;
using KC.Logic.SessionLogic.TableLogic;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.Models.Classes;
using KC.Models.Enums;
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

    public Fin<Unit> CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) => dataStore
        .Add(SessionService.CreateEmptySession(numberOfBoxes, numberOfDecks, timerAfterFirstBet)
            .AddCanBetChangeOnTimerElapsed());

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public Fin<BettingBox> ClaimBox(Guid sessionId, int boxIdx, Player player) => dataStore
        .Get(sessionId)
        .Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not claim boxes at this time.")))
        .Bind(b => b.Claim(player));

    public Fin<BettingBox> DisclaimBox(Guid sessionId, int boxIdx, Player player) => dataStore
        .Get(sessionId)
        .Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not disclaim boxes at this time.")))
        .Bind(b => b.Disclaim(player));

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// </summary>
    public Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount) => dataStore
        .Get(sessionId)
        .Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : FinFail<BettingBox>(Error.New("Can not place bets at this time.")))
        .Bind(b => b.PlaceBet(player, amount));

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Fin of bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public Fin<bool> UpdateTimer(Guid sessionId) => dataStore
        .Get(sessionId)
        .Map(s =>
        {
            if (s.Table.Boxes.Any(b => b.Hands[0].Bet > 0) && !s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Start();
            else if (s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Stop();

            return s.BetPlacementTimer.Enabled;
        });

    public Fin<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId) => dataStore
        .Get(sessionId)
        .Map(s => (s.CurrentBoxIdx, s.CurrentHandIdx));

    
    //REWRITE LATER
    public Fin<Unit> StartDealing(Guid sessionId) => dataStore.Get(sessionId).Map(session =>
        {
            //if shoe needs shuffling, shuffle
            if (session.Table.DealingShoe.ShuffleCardIdx <= session.Table.DealingShoe.NextCardIdx)
            {
                session.Table.DealingShoe.Shuffle(Random.Shared);
            }

            //deal cards
            for (int i = 0; i < 2; i++)
            {
                foreach (BettingBox box in session.Table.BoxesInPlay())
                {
                    box.Hands[i].Cards.Add(session.Table.DealingShoe.TakeCard());
                }
            }

            return Unit.Default;
        });

    public Fin<Seq<Move>> GetPossibleActions(Guid sessionId, int boxIdx, int handIdx) => dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx))
        .Bind<Hand>(b => b.Hands.ElementAtOrDefault(handIdx))
        .Bind(h => h.GetPossibleActions());

    private Fin<Hand> GetHandWithOwnerValidation(Guid sessionId, int boxIdx, int handIdx, Player player) => dataStore
        .Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx)).Bind(b => b.CheckOwner(player))
        .Bind<Hand>(b => b.Hands.ElementAtOrDefault(handIdx));

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions on a hand.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public void MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move) =>
        GetHandWithOwnerValidation(sessionId, boxIdx, handIdx, player)
            .Bind(h => h.GetPossibleActions().Map(a => (hand: h, actionPossible: a.Any(x => x == move))))
            .Bind(a => a.actionPossible 
                ? a.hand
                : FinFail<Hand>(Error.New("Can not make this move on this hand.")))
            .Bind(h => ExecuteMove(sessionId, boxIdx, handIdx, move));

    private Fin<Hand> ExecuteMove(Guid sessionId, int boxIdx, int handIdx, Move move)
    {
        throw new NotImplementedException();
    }

    //transferturn -> if everyone has played, fire and forget endround

    //endround (if everyone has played)

    public Fin<Unit> ResetSession(Guid sessionId) => dataStore.Get(sessionId).Bind(s => s.Table.Reset());
}