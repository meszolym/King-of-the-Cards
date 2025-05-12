using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;

namespace KC.Backend.Logic.Logics.Interfaces;

public interface IGamePlayLogic
{
    /// <summary>
    /// Shuffles the shoe of the table in the session.
    /// </summary>
    void Shuffle(Guid sessionId, Random? random = null);

    /// <summary>
    /// Gets if move is possible on a given hand on a given box.
    /// </summary>
    /// <returns></returns>
    bool CanMakeMove(Guid sessionId, int boxIdx, int handIdx, Move move);

    /// <summary>
    /// Shuffles the shoe of the table in the session if the shuffle card was reached.
    /// </summary>
    bool ShuffleIfNeeded(Guid sessionId, Random? random = null);

    /// <summary>
    /// Gives a card from the shoe of the session.
    /// </summary>
    /// <returns></returns>
    // public Card TakeCardFromShoe(Guid sessionId)
    // {
    //     var session = sessions.Single(s => s.Id == sessionId);
    //     return ;
    // }
    Task AddCardToHand(Guid sessionId, int boxIdx, int handIdx);

    /// <summary>
    /// Plays dealer's hand according to the rules.
    /// </summary>
    /// <exception cref="InvalidOperationException">"It's not the dealer's turn."</exception>
    /// <exception cref="InvalidOperationException">"Dealer's hand is already finished."</exception>
    Task DealerPlayHand(Guid sessionId, TimeSpan delayBetweenCards);

    /// <summary>
    /// Deals cards to the players and the dealer at the start of a round.
    /// </summary>
    /// <exception cref="InvalidOperationException">Shoe needs shuffling.</exception>
    Task DealStartingCards(Guid sessionId, TimeSpan delayBetweenCards, bool checkShuffle = false);

    /// <summary>
    /// Checks for dealer blackjack.
    /// </summary>
    /// <returns>True if the dealer has blackjack</returns>
    bool DealerCheck(Guid sessionId);

    IEnumerable<Move> GetPossibleActionsOnHand(Hand hand);
    IEnumerable<Move> GetPossibleActionsOnHand(Guid sessionId, int boxIdx, int handIdx = 0);

    /// <summary>
    /// Makes a move on a given hand of a given player on a given box. Does not handle player balance, hand bets or transferring turns.
    /// </summary>
    /// <exception cref="InvalidOperationException">"The hand is not in turn."</exception>
    /// <exception cref="InvalidOperationException">"Box is not owned by player."</exception>
    /// <exception cref="InvalidOperationException">"Action not possible." if the rulebook states that this action is not possible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If move is not handled.</exception>
    Task MakeMove(Guid sessionId, int boxIdx, MacAddress playerId, Move move, int handIdx = 0);

    Task TransferTurn(Guid sessionId);
    void FinishAllHandsInPlay(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets TO THE BOXES.
    /// Make sure to handle player balance changes.
    /// </summary>
    Task PayOutBets(Guid sessionId);

    Task ClearHands(Guid sessionId);
}