using KC.App.Logic.Interfaces;
using KC.App.Logic.SessionLogic;
using KC.App.Logic.SessionLogic.BettingBoxLogic;
using KC.App.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Validations;
using Timer = System.Timers.Timer;

namespace KC.App.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IHubContext<SignalRHub> signalRHub) : Controller
{
    private const int numberOfBoxes = 6;
    private const int numberOfDecks = 6;
    private const int secondsToPlaceBets = 15;
    private const int minutesToPurgeOldSessions = 10;
    private const int secondsToPlay = 60;
    private const int secondsToPause = 2;

    [HttpGet("{Id:guid}")]
    public Session GetSession(Guid Id) => sessionLogic.Get(Id);

    [HttpGet]
    public IEnumerable<Session> GetAllSessions()
    {
        if (sessionLogic.PurgeOldSessions(TimeSpan.FromMinutes(minutesToPurgeOldSessions)))
            signalRHub.Clients.All.SendAsync("PurgeOldSessions");

        return sessionLogic.GetAllSessions();
    }

    [HttpPost]
    public Session CreateSession()
    {
        var timer = new TickingTimer(TimeSpan.FromSeconds(secondsToPlaceBets+1)); //+1 sec to account for the time it takes to process everything

        var sess = sessionLogic.CreateSession(numberOfBoxes, numberOfDecks, timer);

        timer.Elapsed += (sender, args) => BettingTimerElapsed(sess);
        timer.Tick += (sender, args) => signalRHub.Clients.Group(sess.Id.ToString()).SendAsync("TimerTick", timer.RemainingSeconds); //or something like that

        signalRHub.Clients.All.SendAsync("SessionCreated", sess);

        return sess;
    }

    private void BettingTimerElapsed(Session sess)
    {
        //SignalR call to those in the session that the timer has elapsed
        sess.DealStartingCards();
        //SignalR call to those in the session that the dealing is done
        if (!sess.DealerCheck())
        {
            //SignalR call to the next player to play
            return;
        }

        sess.FinishAllHandsInPlay();
        //SignalR call to those in the session that the dealer has blackjack
        sess.PayOutBets();
        //SignalR call to those in the session that the payouts are done
        sess.ResetSession();
        //SignalR call to those in the session that the session has been reset
    }

    [HttpPost("{sessionId:guid}/JoinWatch/{playerId}")]
    public void JoinSession(Guid sessionId, string playerId)
    {
        var player = playerLogic.Get(playerId);
        //SignalR subscribe to the session
    }

    [HttpPost("{sessionId:guid}/{boxIdx:int}/ClaimBox/{playerId}")]
    public void ClaimBox(Guid sessionId, int boxIdx, string playerId)
    {
        var player = playerLogic.Get(playerId);
        sessionLogic.Get(sessionId).ClaimBox(boxIdx, player);
        //SignalR subscribe to the box
        //SignalR call to those in the session
    }

    [HttpDelete("{sessionId:guid}/{boxIdx:int}/ClaimBox/{playerId}")]
    public void DisclaimBox(Guid sessionId, int boxIdx, string playerId)
    {
        var player = playerLogic.Get(playerId);
        sessionLogic.Get(sessionId).DisclaimBox(boxIdx, player);
        //SignalR call to those in the session
    }

    [HttpPut("{sessionId:guid}/UpdateBet/{boxIdx:int}/{playerId}/{amount:double}")]
    public void Bet(Guid sessionId, int boxIdx, string playerId, double amount)
    {
        var player = playerLogic.Get(playerId);
        var sess = sessionLogic.Get(sessionId);
        sess.UpdateBet(boxIdx, player, amount);
        var timerOn = sess.UpdateTimer();
        //SignalR call to those in the session that the timer has started/stopped
    }


}