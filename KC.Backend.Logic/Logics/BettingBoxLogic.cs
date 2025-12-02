using System;
using System.Collections.Generic;
using System.Linq;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics;

//This is done.
public class BettingBoxLogic(IList<Session> sessions, IDictionary<MacAddress, Guid> macToPlayerGuid) : IBettingBoxLogic
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
        if (box.OwnerId != Guid.Empty ) throw new InvalidOperationException("Box already has an owner.");
        box.OwnerId = macToPlayerGuid[playerId];
        session.DestructionTimer.Reset();
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
        if (box.OwnerId != macToPlayerGuid[playerId]) throw new InvalidOperationException("Box is not owned by player.");
        box.OwnerId = Guid.Empty;
        session.DestructionTimer.Reset();
    }
    
    
    /// <summary>
    /// Updates the bet on a box of a given player. Does not handle player balance.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <param name="amount"></param>
    /// <exception cref="InvalidOperationException">"Cannot place bets at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player." if the box is nt owned by the player.</exception>
    /// <exception cref="ArgumentException">"Bet cannot be less than 0." if the amount is less than 0.</exception>
    public void UpdateBetOnBox(Guid sessionId, int boxIdx, MacAddress playerId, double amount)
    {
        var session = sessions.Single(s => s.Id == sessionId);
        if (!session.CanPlaceBets) throw new InvalidOperationException("Cannot place bets at this time.");
        var box = session.Table.BettingBoxes[boxIdx];
        if (amount < 0) throw new ArgumentException("Bet cannot be less than 0.");
        
        if (box.OwnerId != macToPlayerGuid[playerId]) throw new InvalidOperationException("Box is not owned by player.");

        box.Hands[0].Bet = amount;

        session.DestructionTimer.Reset();
    }

    public double GetBetOnBox(Guid sessionId, int boxIdx) 
        => sessions.Single(s => s.Id == sessionId).Table.BettingBoxes[boxIdx].Hands[0].Bet;
    
    public BettingBox Get(Guid sessionId, int boxIdx) => sessions.Single(s => s.Id == sessionId).Table.BettingBoxes[boxIdx];

}