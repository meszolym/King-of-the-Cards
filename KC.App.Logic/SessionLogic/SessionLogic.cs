using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection.Metadata.Ecma335;
using KC.App.Data;
using KC.App.Logic.SessionLogic.TableLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.App.Models.Classes;
using KC.App.Models.Enums;
using KC.App.Models.Structs;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using Timer = System.Timers.Timer;
using Unit = LanguageExt.Unit;

namespace KC.App.Logic.SessionLogic;

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

    
    //TODO: REWRITE LATER
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
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public void MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move) =>
        GetHandWithOwnerValidation(sessionId, boxIdx, handIdx, player)
            .Bind(h => h.GetPossibleActions().Bind(possibleMoves => possibleMoves.Any(m => m == move)
                ? ExecuteMove(sessionId, boxIdx, handIdx, move)
                : FinFail<Hand>(Error.New("Can not make this move on this hand."))));

    //TODO: REWRITE LATER
    private Fin<Hand> ExecuteMove(Guid sessionId, int boxIdx, int handIdx, Move move) => dataStore.Get(sessionId)
        .Bind(s => s.Table.GetBettingBox(boxIdx).Map(b => (box: b, session: s))).Map(
            tupBS =>
            {
                switch (move)
                {
                    case Move.Stand:
                        tupBS.box.Hands[handIdx].Finished = true;
                        break;
                    case Move.Hit:
                        tupBS.box.Hands[handIdx].Cards.Add(tupBS.session.Table.DealingShoe.TakeCard());
                        break;
                    case Move.Double:
                        tupBS.box.Hands[handIdx].Bet *= 2;
                        tupBS.box.Hands[handIdx].Cards.Add(tupBS.session.Table.DealingShoe.TakeCard());
                        tupBS.box.Hands[handIdx].Finished = true;
                        break;
                    case Move.Split:
                        tupBS.box.Hands.Insert(handIdx+1, new Hand([tupBS.box.Hands[handIdx].Cards[1]],
                            tupBS.box.Hands[handIdx].Bet, false));

                        tupBS.box.Hands[handIdx].Cards.RemoveAt(1);
                        tupBS.box.Hands[handIdx].Cards.Add(tupBS.session.Table.DealingShoe.TakeCard());
                        tupBS.box.Hands[handIdx].Splittable = false;
                        break;
                }

                return tupBS.box.Hands[handIdx];
            });


    //transferturn -> if everyone has played, fire and forget endround


    public Fin<Hand> TransferTurn(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId).Match(
            Succ: s => s,
            Fail: e => throw new Exception(e.Message));

        var box = sess.Table.GetBettingBox(sess.CurrentBoxIdx).Match(
            Succ: b => b,
            Fail: e => throw new Exception(e.Message));

        box.Hands[sess.CurrentHandIdx].Finished = true;

        if (box.Hands.Count > sess.CurrentHandIdx + 1)
        {
            sess.CurrentHandIdx++;

            //(has to handle giving out a card on the second hand of a split)
            if (box.Hands[sess.CurrentHandIdx].Cards.Count == 1)
            {
                box.Hands[sess.CurrentHandIdx].Cards.Add(sess.Table.DealingShoe.TakeCard());
            }
            return box.Hands[sess.CurrentHandIdx];
        }

        var nextBox = sess.Table.BoxesInPlay().Find(b => b.Hands.Any(h => !h.Finished) && b.Idx > box.Idx).Match(
            Some: b => b,
            None: () => throw new NotImplementedException()); //TODO: endround

        sess.CurrentBoxIdx = nextBox.Idx;
        sess.CurrentHandIdx = 0;

        return nextBox.Hands[0];

    }

    //endround (if everyone has played)
    public Fin<Unit> EndTurn()
    {
        throw new NotImplementedException();
    }

    public Fin<Unit> ResetSession(Guid sessionId) => dataStore.Get(sessionId).Bind(s => s.Table.Reset());
}