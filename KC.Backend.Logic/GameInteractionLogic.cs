using KC.Backend.Logic.GameItemsLogic;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

public class GameInteractionLogic(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IRuleBook ruleBook)
{
    public void ClaimBox(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        session.Table.BettingBoxes[boxIdx].Claim(playerId);
        session.LastMoveMadeAt = DateTime.Now;
    }
    
    public void DisclaimBox(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        session.Table.BettingBoxes[boxIdx].Disclaim(playerId);
        session.LastMoveMadeAt = DateTime.Now;
    }

    /// <summary>
    /// Updates the bet on the given box and hand. Does not handle player balance.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <param name="amount"></param>
    /// <param name="handIdx"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void UpdateBet(Guid sessionId, int boxIdx, Guid playerId, double amount, int handIdx = 0)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        session.Table.BettingBoxes[boxIdx].UpdateBet(playerId, amount, handIdx);
        session.LastMoveMadeAt = DateTime.Now;
        
        UpdateTimer(session);
    }

    private void UpdateTimer(Session session)
    {
        if (session.Table.BettingBoxes.Any(b => b.Hands[0].Bet > 0)
            && !session.BettingTimer.Enabled) session.BettingTimer.Start();
        else if (session.BettingTimer.Enabled) session.BettingTimer.Stop();
    }
    
    public IEnumerable<Move> GetPossibleMoves(Guid sessionId, int boxIdx, Guid playerId, int handIdx = 0)
    {
        var session = sessionLogic.Get(sessionId);
        
        if (!session.CurrentTurnInfo.PlayersTurn 
            || session.CurrentTurnInfo.BoxIdx != boxIdx 
            || session.CurrentTurnInfo.HandIdx != handIdx) throw new InvalidOperationException("This hand is not in turn right now.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];

        var possibleMoves = ruleBook.GetPossibleActionsOnHand(hand);

        if (!possibleMoves.Contains(Move.Double) && !possibleMoves.Contains(Move.Split)) return possibleMoves;
        
        //check if player has enough money to double or split
        var player = playerLogic.Get(playerId);
        if (possibleMoves.Contains(Move.Double) && player.Balance < hand.Bet) possibleMoves.Remove(Move.Double);
        if (possibleMoves.Contains(Move.Split) && player.Balance < hand.Bet) possibleMoves.Remove(Move.Split);

        return possibleMoves;

    }
}