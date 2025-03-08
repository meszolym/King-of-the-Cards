using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

//TODO: Make the logic more atomic, chaining them together will be handled by the API layer.
public class PlayerMoveLogic(IList<Session> sessions, IList<Player> players, IRuleBook ruleBook, IDealerLogic dealerLogic) : IPlayerMoveLogic
{
    /// <summary>
    /// Makes a move on a given hand of a given player on a given box. Does not handle player balance, hand bets or transferring turns.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <param name="move"></param>
    /// <param name="handIdx"></param>
    /// <exception cref="InvalidOperationException">"The hand is not in turn."</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player."</exception>
    /// <exception cref="InvalidOperationException">"Action not possible." if the rulebook states that this action is not possible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If move is not handled.</exception>
    public void MakeMove(Guid sessionId, int boxIdx, Guid playerId, Move move, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CurrentTurnInfo.PlayersTurn || session.CurrentTurnInfo.BoxIdx != boxIdx || session.CurrentTurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand is not in turn.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];
        if (!ruleBook.GetPossibleActionsOnHand(hand).Contains(move)) throw new InvalidOperationException("Action not possible.");
        
        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                hand.Cards.Add(dealerLogic.GiveCard(sessionId));
                break;
            case Move.Double:
                hand.Cards.Add(dealerLogic.GiveCard(sessionId));
                break;
            case Move.Split:
                box.Hands.Add(new Hand(){Cards = new List<Card>(){hand.Cards[1]}, FromSplit = true});
                hand.Cards.RemoveAt(1);
                hand.Cards.Add(dealerLogic.GiveCard(sessionId));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if bust, mark hand as finished
        if (ruleBook.GetValue(hand).NumberValue > 21)
            hand.Finished = true;

        session.LastMoveMadeAt = DateTime.Now;
    }
}