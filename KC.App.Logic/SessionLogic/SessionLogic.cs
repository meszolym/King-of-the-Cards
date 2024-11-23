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
    public void CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet) => dataStore
        .Add(SessionService
            .CreateEmptySession(numberOfBoxes, numberOfDecks, timerAfterFirstBet)
            .AddCanBetChangeOnTimerElapsed());

    public IEnumerable<Session> GetAllSessions() => dataStore.GetAll();

    public void ClaimBox(Guid sessionId, int boxIdx, Player player)
    {
        var sess = dataStore.Get(sessionId);
        if (!sess.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        sess.Table.Boxes[boxIdx].Claim(player);
    }

    public void DisclaimBox(Guid sessionId, int boxIdx, Player player)
    {
        var sess = dataStore.Get(sessionId);
        if (!sess.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        sess.Table.Boxes[boxIdx].Disclaim(player);
    }

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    public void PlaceBet(Guid sessionId, int boxIdx, Player player, double amount)
    {
        var sess = dataStore.Get(sessionId);
        if (!sess.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        sess.Table.Boxes[boxIdx].PlaceBet(player, amount);
    }

    public double GetBetOnBox(Guid sessionId, int boxIdx) => GetBetOnHand(sessionId, boxIdx, 0);

    public double GetBetOnHand(Guid sessionId, int boxIdx, int handIdx) =>
        dataStore.Get(sessionId).Table.Boxes[boxIdx].Hands[handIdx].Bet;

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public bool UpdateTimer(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId);
        if (sess.Table.Boxes.Any(b => b.Hands[0].Bet > 0) && !sess.BetPlacementTimer.Enabled) sess.BetPlacementTimer.Start();
        else if (sess.BetPlacementTimer.Enabled) sess.BetPlacementTimer.Stop();

        return sess.BetPlacementTimer.Enabled;
    }

    public TurnInfo? GetCurrentTurn(Guid sessionId) => dataStore.Get(sessionId).TurnInfo;

    public void StartDealing(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId);

        //if shoe needs shuffling, shuffle
        if (sess.Table.DealingShoe.ShuffleCardIdx <= sess.Table.DealingShoe.NextCardIdx)
        {
            sess.Table.DealingShoe.Shuffle(Random.Shared);
        }

        //deal cards
        for (int i = 0; i < 2; i++)
        {
            foreach (BettingBox box in sess.Table.BoxesInPlay())
            {
                box.Hands[0].Cards.Add(sess.Table.DealingShoe.TakeCard());
            }
            sess.Table.DealerHand.Cards.Add(sess.Table.DealingShoe.TakeCard());
        }
    }

    public IEnumerable<Move> GetPossibleActions(Guid sessionId, int boxIdx, int handIdx) => dataStore.Get(sessionId).Table.Boxes[boxIdx].Hands[handIdx].GetPossibleActions();

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes (eg. split, double).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public void MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move)
    {
        var sess = dataStore.Get(sessionId);
        var box = sess.Table.Boxes[boxIdx];
        var hand = box.Hands[handIdx];

        if (!box.CheckOwner(player)) throw new InvalidOperationException("Player does not own this box.");
        if (!hand.GetPossibleActions().Contains(move)) throw new InvalidOperationException("Action not possible.");

        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                hand.Cards.Add(sess.Table.DealingShoe.TakeCard());
                break;
            case Move.Double:
                hand.Cards.Add(sess.Table.DealingShoe.TakeCard());
                hand.Bet *= 2;
                break;
            case Move.Split:
                box.Hands.Insert(handIdx + 1, new Hand([box.Hands[handIdx].Cards[1]], box.Hands[handIdx].Bet, false));
                hand.Cards.RemoveAt(1);
                hand.Splittable = false;
                hand.Cards.Add(sess.Table.DealingShoe.TakeCard());
                break;
        }
    }

    public TurnInfo TransferTurn(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId);

        #region dealers turn handling
        //if it's the dealer's turn, transfer to the first player's turn
        if (!sess.TurnInfo.PlayersTurn)
        {
            sess.TurnInfo.PlayersTurn = true;
            sess.TurnInfo.BoxIdx = sess.Table.BoxesInPlay().First().Idx;
            sess.TurnInfo.HandIdx = 0;
            return sess.TurnInfo;
        }

        #endregion

        #region hand left handling
        var box = sess.Table.Boxes[sess.TurnInfo.BoxIdx];
        var hand = box.Hands[sess.TurnInfo.HandIdx];

        //mark hand as finished
        hand.Finished = true;

        //if bust, 0 out bet
        if (hand.GetValue().Value > 21) hand.Bet = 0;

        //if there are more hands left in the box, transfer to the next hand
        if (box.Hands.Count > sess.TurnInfo.HandIdx + 1)
        {
            sess.TurnInfo.HandIdx++;
            hand = box.Hands[sess.TurnInfo.HandIdx];

            //handling of split hands, so that they get two cards each
            if (hand.Cards.Count == 1)
            {
                hand.Cards.Add(sess.Table.DealingShoe.TakeCard());
            }

            return sess.TurnInfo;
        }
        #endregion

        #region box left handling
        //if there are more boxes left, transfer to the next box
        var nextBox = sess.Table.BoxesInPlay().FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.Idx > box.Idx);
        if (nextBox is not null)
        {
            sess.TurnInfo.BoxIdx = nextBox.Idx;
            sess.TurnInfo.HandIdx = 0;
            return sess.TurnInfo;
        }
        #endregion

        #region no boxes left handling
        //if there are no more boxes left, transfer to the dealer's turn
        sess.TurnInfo.PlayersTurn = false;
        sess.TurnInfo.BoxIdx = 0;
        sess.TurnInfo.HandIdx = 0;
        return sess.TurnInfo;
        #endregion
    }

    public Hand DealerPlayHand(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId);
        var dealerHand = sess.Table.DealerHand;
        while (dealerHand.GetValue().Value < 17)
        {
            dealerHand.Cards.Add(sess.Table.DealingShoe.TakeCard());
        }
        dealerHand.Finished = true;
        return dealerHand;
    }

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// </summary>
    public void PayOutBets(Guid sessionId)
    {
        var sess = dataStore.Get(sessionId);
        var dealerHand = sess.Table.DealerHand;

        foreach (var box in sess.Table.BoxesInPlay())
        {
            foreach (var hand in box.Hands)
            {
                if (hand.GetValue().Value > 21) //player bust
                {
                    hand.Bet = 0; //lose bet
                    continue;
                }

                if (dealerHand.GetValue().Value >= 21) //dealer bust
                {
                    if (hand.GetValue().IsBlackJack) hand.Bet += hand.Bet*1.5; //if player has blackjack, pay out 1.5x bet
                    else hand.Bet += hand.Bet; //pay out bet
                    continue;
                }

                if (dealerHand.GetValue().IsBlackJack) //dealer has blackjack
                {
                    if (!hand.GetValue().IsBlackJack) hand.Bet = 0; //if player doesn't have blackjack, lose bet, else bet stays the same
                    continue;
                }

                if (hand.GetValue().IsBlackJack) //player has blackjack
                {
                    hand.Bet += hand.Bet * 1.5; //if player has blackjack, pay out 1.5x bet
                }

                if (hand.GetValue().Value > dealerHand.GetValue().Value) //player has stronger hand
                {
                    hand.Bet += hand.Bet; //pay out bet
                    continue;
                }
                
                if (hand.GetValue().Value < dealerHand.GetValue().Value) //player has weaker hand
                {
                    hand.Bet = 0; //lose bet
                }

                //if same value, bet stays the same
            }
        }

    }
    public void ResetSession(Guid sessionId) => dataStore.Get(sessionId).Table.Reset();
}