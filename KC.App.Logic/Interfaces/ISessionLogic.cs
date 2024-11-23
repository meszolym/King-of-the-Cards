using KC.App.Models.Classes;
using KC.App.Models.Enums;
using KC.App.Models.Structs;
using LanguageExt;
using Timer = System.Timers.Timer;

namespace KC.App.Logic.Interfaces;

public interface ISessionLogic
{
    IObservable<TurnInfo> TurnChangedObservable { get; }
    Fin<Unit> CreateSession(uint numberOfBoxes, uint numberOfDecks, Timer timerAfterFirstBet);
    IEnumerable<Session> GetAllSessions();
    Fin<BettingBox> ClaimBox(Guid sessionId, int boxIdx, Player player);
    Fin<BettingBox> DisclaimBox(Guid sessionId, int boxIdx, Player player);

    /// <summary>
    /// Places a bet on a box. Make sure to call UpdateTimer after this to start/stop the timer.
    /// Make sure to take care of player balance changes.
    /// </summary>
    Fin<BettingBox> PlaceBet(Guid sessionId, int boxIdx, Player player, double amount);

    Option<double> GetBetOnBox(Guid sessionId, int boxIdx);
    Option<double> GetBetOnHand(Guid sessionId, int boxIdx, int handIdx);

    /// <summary>
    /// Starts/stops the timer based on whether there are any bets placed.
    /// </summary>
    /// <returns>Fin of bool, if the session exists, the bool represents if the timer is running or not.</returns>
    Fin<bool> UpdateTimer(Guid sessionId);

    Option<(int boxIdx, int handIdx)> GetCurrentTurn(Guid sessionId);
    Fin<Unit> StartDealing(Guid sessionId);
    Option<Seq<Move>> GetPossibleActions(Guid sessionId, int boxIdx, int handIdx);

    /// <summary>
    /// After making a move, make sure to call GetPossibleActions and TransferTurn if there's no more possible actions (except stand) on a hand.
    /// Does not handle player balance changes (eg. split, double).
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="boxIdx"></param>
    /// <param name="handIdx"></param>
    /// <param name="player"></param>
    /// <param name="move"></param>
    Fin<Hand> MakeMove(Guid sessionId, int boxIdx, int handIdx, Player player, Move move);

    Option<Hand> TransferTurn(Guid sessionId);
    Option<Hand> DealerPlayHand(Guid sessionId);

    /// <summary>
    /// Ends the turn, pays out bets to the boxes.
    /// Make sure to handle player balance changes.
    /// 
    /// </summary>
    Option<Unit> EndTurn(Guid sessionId);

    Option<Unit> ResetSession(Guid sessionId);
}