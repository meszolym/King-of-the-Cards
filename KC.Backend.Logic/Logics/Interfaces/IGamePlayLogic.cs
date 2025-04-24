using System;
using System.Net.NetworkInformation;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics.Interfaces;

public interface IGamePlayLogic
{
    /// <summary>
    /// Shuffles the shoe of the table in the session.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="random"></param>
    void Shuffle(Guid sessionId, Random? random = null);

    /// <summary>
    /// Gives a card from the shoe of the session.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    Card GiveCard(Guid sessionId);

    /// <summary>
    /// Plays dealer's hand according to the rules.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <exception cref="InvalidOperationException">"It's not the dealer's turn."</exception>
    /// <exception cref="InvalidOperationException">"Dealer's hand is already finished."</exception>
    void DealerPlayHand(Guid sessionId);

    /// <summary>
    /// Deals cards to the players and the dealer at the start of a round.
    /// </summary>
    /// <param name="sessionId"></param>
    void DealStartingCards(Guid sessionId);

    /// <summary>
    /// Checks for dealer blackjack.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>True if the dealer has blackjack</returns>
    bool DealerCheck(Guid sessionId);

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
    void MakeMove(Guid sessionId, int boxIdx, MacAddress playerId, Move move, int handIdx = 0);
}