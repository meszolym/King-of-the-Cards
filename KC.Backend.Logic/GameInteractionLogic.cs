using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

public class GameInteractionLogic(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IRuleBook ruleBook)
{
    

    
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

    public void DealerPlayHand(Guid sessionId)
    {
        
    }
}