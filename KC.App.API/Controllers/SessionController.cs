using KC.App.Logic.Interfaces;
using KC.App.Logic.SessionLogic;
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

    [HttpGet("{Id}")]
    public Session GetSession(Guid Id) => sessionLogic.Get(Id);

    [HttpGet]
    public IEnumerable<Session> GetAllSessions()
    {
        if (sessionLogic.PurgeOldSessions()) 
            signalRHub.Clients.All.SendAsync("PurgeOldSessions");
        return sessionLogic.GetAllSessions();
    }

    [HttpPost]
    public Session CreateSession()
    {
        var sess = sessionLogic.CreateSession(numberOfBoxes, numberOfDecks, new Timer(TimeSpan.FromSeconds(secondsToPlaceBets)));
        signalRHub.Clients.All.SendAsync("SessionCreated", sess);
        return sess;
    }

    [HttpPost("{sessionId}/{boxIdx}/ClaimBox/{playerId}")]
    public void ClaimBox(Guid sessionId, int boxIdx, string playerId)
    {
        var player = playerLogic.Get(playerId);
        sessionLogic.Get(sessionId).ClaimBox(boxIdx, player);

    }
}