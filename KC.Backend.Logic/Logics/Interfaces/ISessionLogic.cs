using System;
using System.Collections.Generic;
using KC.Backend.Models.GameManagement;

namespace KC.Backend.Logic.Logics.Interfaces;

public interface ISessionLogic
{
    /// <summary>
    /// Creates an empty session. Make sure to subscribe to events of the timers.
    /// </summary>
    /// <param name="numberOfBoxes"></param>
    /// <param name="numberOfDecks"></param>
    /// <param name="shuffleCardPlacement"></param>
    /// <param name="shuffleCardRange"></param>
    /// <param name="bettingTimeSpan"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    Session CreateSession(uint numberOfBoxes, uint numberOfDecks, int shuffleCardPlacement, uint shuffleCardRange, TimeSpan bettingTimeSpan, TimeSpan sessionDestructionTimeSpan, Random? random = null);

    Session RemoveSession(Guid sessId);
    Session Get(Guid sessionId);
    IEnumerable<Session> GetAll();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>Whether the betting timer is enabled after the update.</returns>
    bool UpdateBettingTimer(Guid sessionId);
}