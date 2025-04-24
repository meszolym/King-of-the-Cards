using System;
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
    /// Deals cards to the players and the dealer at the start of a round (only 1 per hand, so THIS HAS TO BE CALLED TWICE).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <exception cref="InvalidOperationException">Shoe needs shuffling.</exception>
    void DealHalfOfStartingCards(Guid sessionId, bool checkShuffle);

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

    void TransferTurn(Guid sessionId);
    void FinishAllHandsInPlay(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    void PayOutBets(Guid sessionId);

    void ClearHands(Guid sessionId);
}