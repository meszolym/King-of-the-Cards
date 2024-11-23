using KC.App.Models.Classes;
using KC.App.Models.Enums;
using Timer = System.Timers.Timer;

namespace KC.App.Logic.Interfaces;

public interface ISessionLogic
{
    void CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet);
    IEnumerable<Session> GetAllSessions();
    void ClaimBox(Guid sessionId, int boxIdx, Player player);
    void DisclaimBox(Guid sessionId, int boxIdx, Player player);

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    void PlaceBet(Guid sessionId, int boxIdx, Player player, double amount);

    double GetBetOnBox(Guid sessionId, int boxIdx);
    double GetBetOnHand(Guid sessionId, int boxIdx, int handIdx);

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Bool, if the session exists, the bool represents if the timer is running or not.</returns>
    bool UpdateTimer(Guid sessionId);

    TurnInfo? GetCurrentTurn(Guid sessionId);
    void StartDealing(Guid sessionId);
    IEnumerable<Move> GetPossibleActions(Guid sessionId, int boxIdx, int handIdx);

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes (eg. split, double).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    void MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move);

    TurnInfo TransferTurn(Guid sessionId);
    Hand DealerPlayHand(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// </summary>
    void PayOutBets(Guid sessionId);

    void ResetSession(Guid sessionId);
}