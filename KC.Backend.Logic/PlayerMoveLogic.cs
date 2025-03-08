using KC.Backend.Logic.Interfaces;
using KC.Backend.Models;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic;

public class PlayerMoveLogic(IList<Session> sessions, IList<Player> players, IRuleBook ruleBook, IDealerLogic dealerLogic) : IPlayerMoveLogic
{
    public IEnumerable<Move> GetPossibleMoves(Guid sessionId, int boxIdx, Guid playerId, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        
        if (!session.CurrentTurnInfo.PlayersTurn 
            || session.CurrentTurnInfo.BoxIdx != boxIdx 
            || session.CurrentTurnInfo.HandIdx != handIdx) throw new InvalidOperationException("This hand is not in turn right now.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];

        var possibleMoves = ruleBook.GetPossibleActionsOnHand(hand);

        if (!possibleMoves.Contains(Move.Double) && !possibleMoves.Contains(Move.Split)) return possibleMoves;
        
        //check if player has enough money to double or split
        var player = players.Single(p => p.Id == playerId);
        if (possibleMoves.Contains(Move.Double) && player.Balance < hand.Bet) possibleMoves.Remove(Move.Double);
        if (possibleMoves.Contains(Move.Split) && player.Balance < hand.Bet) possibleMoves.Remove(Move.Split);

        return possibleMoves;

    }

    /// <summary>
    /// After making a move, make sure to call GetPossibleMoves and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// </summary>
    public void MakeMove(Guid sessionId, int boxIdx, Guid playerId, Move move, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CurrentTurnInfo.PlayersTurn || session.CurrentTurnInfo.BoxIdx != boxIdx || session.CurrentTurnInfo.HandIdx != handIdx)
            throw new InvalidOperationException("This hand is not in turn.");
        
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var hand = box.Hands[handIdx];
        if (!GetPossibleMoves(sessionId, boxIdx, playerId, handIdx).Contains(move)) throw new InvalidOperationException("Action not possible.");
        Player player; //in case of double or split
        switch (move)
        {
            case Move.Stand:
                hand.Finished = true;
                break;
            case Move.Hit:
                hand.Cards.Add(dealerLogic.TakeCard(sessionId));
                break;
            case Move.Double:
                hand.Cards.Add(dealerLogic.TakeCard(sessionId));
                player = players.Single(p => p.Id == playerId);
                player.Balance -= hand.Bet;
                hand.Bet *= 2;
                break;
            case Move.Split:
                player = players.Single(p => p.Id == playerId);
                player.Balance -= hand.Bet;
                box.Hands.Add(new Hand(){Cards = new List<Card>(){hand.Cards[1]}, Bet = hand.Bet, FromSplit = true});
                hand.Cards.RemoveAt(1);
                hand.Cards.Add(dealerLogic.TakeCard(sessionId));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

        //if bust, mark hand as finished
        if (ruleBook.GetValue(hand).NumberValue > 21)
            hand.Finished = true;

        session.LastMoveMadeAt = DateTime.Now;
    }
    
    public void UpdateBetOnBox(Guid sessionId, int boxIdx, Guid playerId, double amount, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (amount < 0) throw new ArgumentException("Bet cannot be less than 0.");
        
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        
        var delta = amount - box.Hands[handIdx].Bet;
        var player = players.Single(p => p.Id == playerId);
        if (player.Balance < delta) throw new InvalidOperationException("Amount required exceeds player balance.");
        
        player.Balance -= delta;
        box.Hands[handIdx].Bet += delta;
        
        session.LastMoveMadeAt = DateTime.Now;
    }
    
    public void ClaimBettingBox(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != Guid.Empty ) throw new InvalidOperationException("Box already has an owner.");
        box.OwnerId = playerId;
        
        session.LastMoveMadeAt = DateTime.Now;
    }

    public void DisclaimBettingBox(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        box.OwnerId = Guid.Empty;
        session.LastMoveMadeAt = DateTime.Now;
    }

}