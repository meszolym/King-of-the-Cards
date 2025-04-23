using System;
using System.Collections.Generic;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Interfaces;

public interface ISessionLogic
{
    /// <summary>
    /// Creates an empty session. Make sure to subscribe to events of the timer.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="shuffleCardPlacement">The placement of the suffle card. If negative, it is counted from the end of the deck backwards.</param>
    /// <param name="shuffleCardRange"></param>
    /// <param name="bettingTimeSpan"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, uint shuffleCardRange, TimeSpan bettingTimeSpan, TimeSpan sessionDestructionTimeSpan, Random? random = null);

    Session Get(Guid sessionId);
    IEnumerable<Session> GetAll();
    Session RemoveSession(Guid sessionId);
    void UpdateTimer(Guid sessionId);
    void TransferTurn(Guid sessionId);
    void FinishAllHandsInPlay(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    void PayOutBets(Guid sessionId);

    void ClearHands(Guid sessionId);
}