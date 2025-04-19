using System;
using System.Net.NetworkInformation;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Interfaces;

public interface IBettingBoxLogic
{
    /// <summary>
    /// Claims box for a given player.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException">"Cannot claim boxes at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box already has an owner." if the box has an owner.</exception>
    void ClaimBettingBox(Guid sessionId, int boxIdx, MacAddress playerId);

    /// <summary>
    /// Disclaims box for a given player.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="playerId"></param>
    /// <exception cref="InvalidOperationException">"Cannot disclaim boxes at this time." if the round is already going.</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player." if the box has an owner that is not the given player.</exception>
    void DisclaimBettingBox(Guid sessionId, int boxIdx, MacAddress playerId);

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
    void UpdateBetOnBox(Guid sessionId, int boxIdx, MacAddress playerId, double amount, int handIdx = 0);
}