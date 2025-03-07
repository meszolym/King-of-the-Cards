using KC.Backend.Logic.Interfaces;

namespace KC.Backend.Logic;
public class BettingBoxLogic(ISessionLogic sessionLogic) : IBettingBoxLogic
{
    public void Claim(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != Guid.Empty )
        {
            throw new InvalidOperationException("Box already has an owner.");
        }
        box.OwnerId = playerId;
        
        session.LastMoveMadeAt = DateTime.Now;
    }

    public void DisclaimBox(Guid sessionId, int boxIdx, Guid playerId)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId)
        {
            throw new InvalidOperationException("Player does not own this box.");
        }
        box.OwnerId = Guid.Empty;
        session.LastMoveMadeAt = DateTime.Now;
    }

    public void UpdateBet(Guid sessionId, int boxIdx, Guid playerId, double amount, int handIdx = 0)
    {
        var session = sessionLogic.Get(sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId)
        {
            throw new InvalidOperationException("Player does not own this box.");
        }

        if (amount < 0)
        {
            throw new ArgumentException("Bet cannot be less than 0.");
        }
        
        var oldBet = box.Hands[handIdx].Bet;
        box.Hands[handIdx].Bet = amount;
        session.LastMoveMadeAt = DateTime.Now;
        
        sessionLogic.UpdateTimer(sessionId);
    }

    public void ClearHands(Guid sessionId, int boxIdx)
    {
        var session = sessionLogic.Get(sessionId);
        var box = session.Table.BettingBoxes[boxIdx];
        box.Hands.Clear();
        box.Hands.Add(new ());
    }

}
