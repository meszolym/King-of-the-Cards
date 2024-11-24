using KC.App.Logic.SessionLogic.TableLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Logic.SessionLogic.TableLogic.ShoeLogic;
using KC.App.Models.Classes;
using KC.App.Models.Classes.Hand;
using KC.App.Models.Enums;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic;
public static class SessionExtensions
{
    public static Session AddCanBetChangeOnTimerElapsed(this Session session)
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
        session.Table.Boxes[boxIdx].Claim(player);
        session.LastMoveAt = DateTime.Now;
    }

    public static void DisclaimBox(this Session session, int boxIdx, Player player)
    {
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        session.Table.Boxes[boxIdx].Disclaim(player);
        session.LastMoveAt = DateTime.Now;
    }

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    public static void PlaceBet(this Session session, int boxIdx, Player player, double amount)
    {
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        session.Table.Boxes[boxIdx].PlaceBet(player, amount);
        session.LastMoveAt = DateTime.Now;
    }

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Bool, if the session exists, the bool represents if the timer is running or not.</returns>
    public static bool UpdateTimer(this Session session)
    {
        if (session.Table.Boxes.Any(b => b.Hands[0].Bet > 0)
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

        session.Table.MakeMove(boxIdx, handIdx, player, move);

        session.LastMoveAt = DateTime.Now;
    }

    public static Hand DealerPlayHand(this Session session)
    {
        if (session.TurnInfo.PlayersTurn) throw new InvalidOperationException("It's not the dealer's turn.");
        return session.Table.DealerPlayHand();
    }

    public static TurnInfo TransferTurn(this Session session)
    {
        #region dealers turn handling
        //if it's the dealer's turn, transfer to the first player's turn
        if (!session.TurnInfo.PlayersTurn)
        {
            session.TurnInfo.PlayersTurn = true;
            session.TurnInfo.BoxIdx = session.Table.BoxesInPlay().First().Idx;
            session.TurnInfo.HandIdx = 0;
            return session.TurnInfo;
        }

        #endregion

        #region hand left handling
        var box = session.Table.Boxes[session.TurnInfo.BoxIdx];
        var hand = box.Hands[session.TurnInfo.HandIdx];

        //mark hand as finished
        hand.Finished = true;

        //if bust, 0 out bet
        if (hand.GetValue().Value > 21) hand.Bet = 0;

        //if there are more hands left in the box, transfer to the next hand
        if (box.Hands.Count > session.TurnInfo.HandIdx + 1)
        {
            session.TurnInfo.HandIdx++;
            hand = box.Hands[session.TurnInfo.HandIdx];

            //handling of split hands, so that they get two cards each
            if (hand.Cards.Count == 1)
            {
                hand.Cards.Add(session.Table.DealingShoe.TakeCard());
            }

            return session.TurnInfo;
        }
        #endregion

        #region box left handling
        //if there are more boxes left, transfer to the next box
        var nextBox = session.Table.BoxesInPlay().FirstOrDefault(b => b.Hands.Any(h => !h.Finished) && b.Idx > box.Idx);
        if (nextBox is not null)
        {
            session.TurnInfo.BoxIdx = nextBox.Idx;
            session.TurnInfo.HandIdx = 0;
            return session.TurnInfo;
        }
        #endregion

        #region no boxes left handling
        //if there are no more boxes left, transfer to the dealer's turn
        session.TurnInfo.PlayersTurn = false;
        session.TurnInfo.BoxIdx = 0;
        session.TurnInfo.HandIdx = 0;
        return session.TurnInfo;
        #endregion
    }

    public static void ResetSession(this Session session) => session.Table.Reset();

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// </summary>
    public static void PayOutBets(this Session session) => session.Table.PayOutBets();

}