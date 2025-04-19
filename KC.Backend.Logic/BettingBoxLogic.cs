using System.Net.NetworkInformation;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic;

//This is done.
public class BettingBoxLogic(IList<Session> sessions) : IBettingBoxLogic
{
    /// <summary>
    /// Claims box for a given player.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException">"Cannot claim boxes at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box already has an owner." if the box has an owner.</exception>
    public void ClaimBettingBox(Guid sessionId, int boxIdx, MacAddress playerId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot claim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != MacAddress.None ) throw new InvalidOperationException("Box already has an owner.");
        box.OwnerId = playerId;

        //session.LastMoveMadeAt = DateTime.Now;
        session.DestructionTimer.Stop();
        session.DestructionTimer.Start();
    }

    /// <summary>
    /// Disclaims box for a given player.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException">"Cannot disclaim boxes at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player." if the box has an owner that is not the given player.</exception>
    public void DisclaimBettingBox(Guid sessionId, int boxIdx, MacAddress playerId)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot disclaim boxes at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");
        box.OwnerId = MacAddress.None;
        //session.LastMoveMadeAt = DateTime.Now;
        session.DestructionTimer.Stop();
        session.DestructionTimer.Start();
    }
    
    
    /// <summary>
    /// Updates the bet on a box of a given player. Does not handle player balance.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <param name="amount"></param>
    /// <param name="handIdx"></param>
    /// <exception cref="InvalidOperationException">"Cannot place bets at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player." if the box is nt owned by the player.</exception>
    /// <exception cref="ArgumentException">"Bet cannot be less than 0." if the amount is less than 0.</exception>
    public void UpdateBetOnBox(Guid sessionId, int boxIdx, MacAddress playerId, double amount, int handIdx = 0)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (amount < 0) throw new ArgumentException("Bet cannot be less than 0.");
        
        if (box.OwnerId != playerId) throw new InvalidOperationException("Box is not owned by player.");

        box.Hands[handIdx].Bet = amount;
        
        //session.LastMoveMadeAt = DateTime.Now;
        session.DestructionTimer.Stop();
        session.DestructionTimer.Start();
    }
    
}