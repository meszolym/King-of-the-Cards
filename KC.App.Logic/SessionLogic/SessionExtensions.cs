using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using KC.App.Logic.CardLogic;
using KC.App.Logic.SessionLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.HandLogic;
using KC.App.Logic.SessionLogic.ShoeLogic;
using KC.App.Models.Classes;
using KC.App.Models.Classes.Hand;
using KC.App.Models.Enums;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic;
public static class SessionExtensions
{
    internal static Session AddCanBetChangeOnTimerElapsed(this Session session)
    {
        session.BetPlacementTimer.Elapsed += (sender, args) =>
        {
            session.CanPlaceBets = false;
            session.BetPlacementTimer.Stop();
        };
        return session;
    }

    public static void ClaimBox(this Session session, int boxIdx, Player player)
    {
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        session.Boxes[boxIdx].Claim(player);
        session.LastMoveAt = DateTime.Now;
    }

    public static void DisclaimBox(this Session session, int boxIdx, Player player)
    {
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        session.Boxes[boxIdx].Disclaim(player);
        session.LastMoveAt = DateTime.Now;
    }

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    public static void UpdateBet(this Session session, int boxIdx, Player player, double amount)
    {
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        session.Boxes[boxIdx].UpdateBet(player, amount);
        session.LastMoveAt = DateTime.Now;
    }

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public static bool UpdateTimer(this Session session)
    {
        if (session.Boxes.Any(b => b.Hands[0].Bet > 0)
            && !session.BetPlacementTimer.Enabled) session.BetPlacementTimer.Start();
        else if (session.BetPlacementTimer.Enabled) session.BetPlacementTimer.Stop();

        return session.BetPlacementTimer.Enabled;
    }

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes (eg. split, double).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    public static void MakeMove(this Session session, int boxIdx, int handIdx, Player player, Move move)
    {

        if (!session.TurnInfo.PlayersTurn || session.TurnInfo.BoxIdx != boxIdx || session.TurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand or box is not in turn.");

        var box = session.Boxes[boxIdx];
        var hand = box.Hands[handIdx];
        if (!box.CheckOwner(player))
            throw new InvalidOperationException("Player does not own this box.");
        if (!hand.GetPossibleActions().Contains(move))
            throw new InvalidOperationException("Action not possible.");

        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                hand.Cards.Add(session.DealingShoe.TakeCard());
                break;
            case Move.Double:
                hand.Cards.Add(session.DealingShoe.TakeCard());
                hand.Bet *= 2;
                break;
            case Move.Split:
                box.Hands.Insert(handIdx + 1,
                    new PlayerHand([box.Hands[handIdx].Cards[1]],
                        box.Hands[handIdx].Bet,
                        false));
                hand.Cards.RemoveAt(1);
                hand.Splittable = false;
                hand.Cards.Add(session.DealingShoe.TakeCard());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if bust, mark hand as finished
        if (hand.GetValue().Value > 21)
            hand.Finished = true;

        session.LastMoveAt = DateTime.Now;
    }

    public static Hand DealerPlayHand(this Session session)
    {
        if (session.TurnInfo.PlayersTurn) throw new InvalidOperationException("It's not the dealer's turn.");
        session.DealerHand.ShowsCards = true;

        while (session.DealerHand.GetValue().Value < 17)
        {
            session.DealerHand.Cards.Add(session.DealingShoe.TakeCard());
            //call some event or something to show the card?
        }
        session.DealerHand.Finished = true;
        return session.DealerHand;
    }

    public static TurnInfo TransferTurn(this Session session)
    {
        #region dealers turn handling
        //if it's the dealer's turn, transfer to the first player's turn
        if (!session.TurnInfo.PlayersTurn)
        {
            session.TurnInfo = new TurnInfo(true, session.BoxesInPlay().First().Idx, 0);
            return session.TurnInfo;
        }

        #endregion

        #region hand left handling
        var box = session.Boxes[session.TurnInfo.BoxIdx];
        var hand = box.Hands[session.TurnInfo.HandIdx];

        //mark hand as finished
        hand.Finished = true;

        //if bust, 0 out bet
        if (hand.GetValue().Value > 21) hand.Bet = 0;

        //if there are more hands left in the box, transfer to the next hand
        if (box.Hands.Count > session.TurnInfo.HandIdx + 1)
        {
            session.TurnInfo = session.TurnInfo with { HandIdx = session.TurnInfo.HandIdx + 1 };
            hand = box.Hands[session.TurnInfo.HandIdx];

            //handling of split hands, so that they get two cards each
            if (hand.Cards.Count == 1)
            {
                hand.Cards.Add(session.DealingShoe.TakeCard());
            }

            return session.TurnInfo;
        }
        #endregion

        #region box left handling
        //if there are more boxes left, transfer to the next box
        var nextBox = session.BoxesInPlay().FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.Idx > box.Idx);
        if (nextBox is not null)
        {
            session.TurnInfo = session.TurnInfo with { BoxIdx = nextBox.Idx, HandIdx = 0 };
            return session.TurnInfo;
        }
        #endregion

        #region no boxes left handling
        //if there are no more boxes left, transfer to the dealer's turn
        session.TurnInfo = new TurnInfo(); //false, 0, 0 by default
        return session.TurnInfo;
        #endregion
    }

    public static void ResetSession(this Session session) => session.Boxes.ForEach(b => b.ClearHands());

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// </summary>
    public static void PayOutBets(this Session session)
    {
        var dealerHand = session.DealerHand;

        foreach (var box in session.BoxesInPlay())
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
                    if (hand.GetValue().IsBlackJack) hand.Bet += hand.Bet * 1.5; //if player has blackjack, pay out 1.5x bet
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

    public static IEnumerable<BettingBox> BoxesInPlay(this Session sess) =>
        sess.Boxes.Where(box => box.Hands[0].Bet > 0).OrderBy(b => b.Idx);

    public static void DealStartingCards(this Session session)
    {
        //if shoe needs shuffling, shuffle
        if (session.DealingShoe.ShuffleCardIdx <= session.DealingShoe.NextCardIdx)
        {
            session.DealingShoe.Shuffle(Random.Shared);
        }

        //deal cards
        for (int i = 0; i < 2; i++)
        {
            foreach (BettingBox box in session.BoxesInPlay())
            {
                box.Hands[0].Cards.Add(session.DealingShoe.TakeCard());
            }
            session.DealerHand.Cards.Add(session.DealingShoe.TakeCard());
        }
    }

    public static bool DealerCheck(this Session session)
    {
        if (!session.DealerHand.GetValue().IsBlackJack) return false;
        session.DealerHand.ShowsCards = true;
        session.DealerHand.Finished = true;
        return true;
    }

    public static void FinishAllHandsInPlay(this Session session)
    {
        foreach (var b in session.BoxesInPlay())
        {
            foreach (var h in b.Hands)
            {
                h.Finished = true;
            }
        }
    }

}