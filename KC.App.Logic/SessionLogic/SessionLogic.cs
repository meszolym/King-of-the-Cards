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
using KC.App.Models.Structs;
using Timer = System.Timers.Timer;


namespace KC.App.Logic.SessionLogic;

public class SessionLogic(IDataStore<Session, Guid> dataStore)
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
            : Option<BettingBox>.None)
        .ToFin(Error.New("Can not claim boxes at this time."))
        .Bind(b => b.Claim(player));

    public Fin<BettingBox> DisclaimBox(Guid sessionId, int boxIdx, Player player) => dataStore
        .Get(sessionId)
        .Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : Option<BettingBox>.None)
        .ToFin(Error.New("Can not disclaim boxes at this time."))
        .Bind(b => b.Disclaim(player));

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    public Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount) => dataStore
        .Get(sessionId)
        .Bind(s => s.CanPlaceBets
            ? s.Table.GetBettingBox(boxIdx)
            : Option<BettingBox>.None)
        .ToFin(Error.New("Can not place bets at this time."))
        .Bind(b => b.PlaceBet(player, amount));

    public Option<double> GetBetOnBox(Guid sessionId, int boxIdx) => GetBetOnHand(sessionId, boxIdx, 0);

    public Option<double> GetBetOnHand(Guid sessionId, int boxIdx, int handIdx) => dataStore
        .Get(sessionId)
        .Bind(s => s.Table.GetBettingBox(boxIdx))
        .Bind<Hand>(b => b.Hands.ElementAtOrDefault(handIdx))
        .Map(h => h.Bet);

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Fin of bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public Fin<bool> UpdateTimer(Guid sessionId) => dataStore
        .Get(sessionId).ToFin(Error.New("Session does not exist."))
        .Map(s =>
        {
            if (s.Table.Boxes.Any(b => b.Hands[0].Bet > 0) && !s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Start();
            else if (s.BetPlacementTimer.Enabled) s.BetPlacementTimer.Stop();

            return s.BetPlacementTimer.Enabled;
        });

    public Option<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId) => dataStore
        .Get(sessionId)
        .Map(s => (s.CurrentBoxIdx, s.CurrentHandIdx));

    
    //TODO: REWRITE LATER
    public Fin<Unit> StartDealing(Guid sessionId) => dataStore
        .Get(sessionId)
        .ToFin(Error.New("Cannot find session"))
        .Map(session =>
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
                session.Table.DealerHand.Cards.Add(session.Table.DealingShoe.TakeCard());
            }

            return Unit.Default;
        });

    public Option<Seq<Move>> GetPossibleActions(Guid sessionId, int boxIdx, int handIdx) => dataStore.Get(sessionId).Bind(s => s.Table.GetBettingBox(boxIdx))
        .Bind<Hand>(b => b.Hands.ElementAtOrDefault(handIdx))
        .Bind(h => h.GetPossibleActions());

    private Fin<Hand> GetHandWithOwnerValidation(Guid sessionId, int boxIdx, int handIdx, Player player) => dataStore
        .Get(sessionId)
        .Bind(s => s.Table.GetBettingBox(boxIdx))
        .ToFin(Error.New("Session cannot be found."))
        .Bind(b => b.CheckOwner(player))
        .Bind<Hand>(b => b.Hands.ElementAtOrDefault(handIdx));

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes (eg. split, double).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public Fin<Hand> MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move) =>
        GetHandWithOwnerValidation(sessionId, boxIdx, handIdx, player)
            .Bind(h => h.GetPossibleActions().ToFin(Error.New("No possible actions found")))
            .Bind(possibleMoves => 
                possibleMoves.Any(m => m == move)
                ? ExecuteMove(sessionId, boxIdx, handIdx, move).ToFin(Error.New("Action not possible"))
                : Error.New("Action not possible"));

    //TODO: REWRITE LATER
    private Option<Hand> ExecuteMove(Guid sessionId, int boxIdx, int handIdx, Move move) => dataStore.Get(sessionId)
        
        .Bind(s => s.Table.GetBettingBox(boxIdx).Map(b => (sess: s, box: b))).Bind(
            tupSB => move switch
            {
                Move.Stand => tupSB.box.Hands[handIdx].Finish(),
                Move.Hit => tupSB.box.Hands[handIdx].AddCard(tupSB.sess.Table.DealingShoe.TakeCard()),
                Move.Double => tupSB.box.Hands[handIdx].Double(tupSB.sess.Table.DealingShoe.TakeCard()),
                Move.Split => tupSB.box.Split(handIdx, tupSB.sess.Table.DealingShoe.TakeCard()),
                _ => Option<Hand>.None
            });


    //transferturn -> if everyone has played, call dealerPlayHand
    //Todo: REWRITE LATER
    public Option<Hand> TransferTurn(Guid sessionId) => dataStore.Get(sessionId)
        .Bind(s => s.Table.GetBettingBox(s.CurrentBoxIdx).Map(b =>
        {
            b.Hands[s.CurrentHandIdx].Finish();
            return (sess: s, box: b);
        })).Bind(tupSB =>
        {
            //if bust, 0 out bet
            if (tupSB.box.Hands[tupSB.sess.CurrentHandIdx].GetValue().Value > 21)
            {
                tupSB.box.Hands[tupSB.sess.CurrentHandIdx].Bet = 0;
            }

            if (tupSB.box.Hands.Count > tupSB.sess.CurrentHandIdx + 1)
            {
                tupSB.sess.CurrentHandIdx++;

                //handling of split hands, so that they get two cards each
                if (tupSB.box.Hands[tupSB.sess.CurrentHandIdx].Cards.Count == 1)
                {
                    tupSB.box.Hands[tupSB.sess.CurrentHandIdx].Cards.Add(tupSB.sess.Table.DealingShoe.TakeCard());
                }

                return tupSB.box.Hands[tupSB.sess.CurrentHandIdx];
            }

            return ((Option<BettingBox>)tupSB.sess.Table.BoxesInPlay()
                .FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.Idx > tupSB.box.Idx)).Map(b =>
                {
                    tupSB.sess.CurrentBoxIdx = b.Idx;
                    tupSB.sess.CurrentHandIdx = 0;
                    return b.Hands[0];
                });
        });

    //TODO: Rewrite later
    public Option<Hand> DealerPlayHand(Guid sessionId) => dataStore.Get(sessionId).Map(s =>
    {
        //dealer plays
        while (s.Table.DealerHand.GetValue().Value < 17)
        {
            s.Table.DealerHand.Cards.Add(s.Table.DealingShoe.TakeCard());
        }
        s.Table.DealerHand.Finished = true;
        return s.Table.DealerHand;
    });

    //endround (if everyone and the dealer has played)
    //TODO: REWRITE LATER
    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// 
    /// </summary>
    public Option<Unit> EndTurn(Guid sessionId) => dataStore.Get(sessionId).Map(s =>
        (dh: s.Table.DealerHand, boxes: s.Table.BoxesInPlay())).Map(
        tupDB => //switchify?
        {
            var dhVal = tupDB.dh.GetValue();
            if (dhVal.Value > 21) //dealer bust
            {
                tupDB.boxes.AsIterable().Iter(b => b.Hands.ForEach(h =>
                {
                    var hVal = h.GetValue();
                    if (hVal.Value <= 21) //if player not bust, pay out bet
                    {
                        h.Bet += h.Bet;

                        if (hVal.IsBlackJack) //if player has blackjack, pay out 1.5x bet
                        {
                            h.Bet += h.Bet / 2;
                        }
                    }
                    else
                    {
                        h.Bet = 0;
                    }
                }));
                return Unit.Default;
            }
            
            if (dhVal.IsBlackJack) //dealer blackjack
            {
                tupDB.boxes.AsIterable().Iter(b => b.Hands.ForEach(h =>
                {
                    if (!h.GetValue().IsBlackJack)
                    {
                        h.Bet = 0;
                    }
                }));
                return Unit.Default;
            }

            //dealer not bust, not blackjack
            tupDB.boxes.AsIterable().Iter(b => b.Hands.ForEach(h =>
            {
                var hVal = h.GetValue();
                if (hVal.Value <= 21) //if player not bust
                {
                    if (hVal.Value > dhVal.Value) //if player has higher value
                    {
                        h.Bet += h.Bet;
                        if (hVal.IsBlackJack) //if player has blackjack, pay out 1.5x bet
                        {
                            h.Bet += h.Bet / 2;
                        }
                    }
                    else if (hVal.Value != dhVal.Value) //if player has lower value (same value means bet doesn't change)
                    {
                        h.Bet = 0;
                    }
                }
                else //if player bust
                {
                    h.Bet = 0;
                }
            }));
            return Unit.Default;
        });
    public Option<Unit> ResetSession(Guid sessionId) => dataStore.Get(sessionId).Map(s => s.Table.Reset());
}