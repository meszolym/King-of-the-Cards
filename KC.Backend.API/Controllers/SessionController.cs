using System.Diagnostics;
using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Backend.Models.GameManagement;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IClientCommunicator hub,
    GetPlayerNameDelegate getPlayerName, OnDestructionTimerElapsedDelegate onDestructionTimerElapsed,
    OnBettingTimerTickedDelegate onBettingTimerTicked, OnBettingTimerElapsedDelegate onBettingTimerElapsed, OnTurnInfoChangedDelegate onTurnInfoChanged) : Controller
{
    [HttpGet]
    public IEnumerable<SessionReadDto> GetAllSessions() => sessionLogic.GetAll().Select(s => s.ToDto(getPlayerName));

    [HttpGet("{id:guid}")]
    public SessionReadDto GetSession(Guid id) =>
        sessionLogic.Get(id).ToDto(getPlayerName);

    [HttpPost("join/{sessionId:guid}")]
    public async Task JoinSession([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, Guid sessionId)
    {
        var mac = MacAddress.Parse(macAddress);
        var connId = playerLogic.Get(mac).ConnectionId;
        await hub.MoveToGroupAsync(connId, sessionId);
        
        var sess = sessionLogic.Get(sessionId);
        sess.DestructionTimer.Reset();
        
    }

    [HttpDelete("leave/{sessionId:guid}")]
    public async Task LeaveSession([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, Guid sessionId)
    {
        var connId = playerLogic.Get(MacAddress.Parse(macAddress)).ConnectionId;
        await hub.MoveToGroupAsync(connId, hub.BaseGroup);
    }
    
    private const uint DefaultBoxes = 5;
    private const uint DefaultDecks = 6;
    private const int DefaultShuffleCardPlacement = -40;
    private const uint DefaultShuffleCardRange = 10;
    private const uint DefaultBettingTimeSpanSecs = 10;
    private const uint DefaultSessionDestructionTimeSpanSecs = 5*60; // 5 minutes

    [HttpPost]
    [Route("create")]
    public async Task CreateSession()
    {
        var sess = sessionLogic.CreateSession(DefaultBoxes, DefaultDecks, DefaultShuffleCardPlacement, DefaultShuffleCardRange, TimeSpan.FromSeconds(DefaultBettingTimeSpanSecs), TimeSpan.FromSeconds(DefaultSessionDestructionTimeSpanSecs));
        
        sess.DestructionTimer.Elapsed += async (sender, args) => await onDestructionTimerElapsed(sess.Id);
        sess.BettingTimer.Tick += async (sender, args) => await onBettingTimerTicked(sess.Id, sess.BettingTimer.RemainingSeconds);
        sess.BettingTimer.Elapsed += async (sender, args) => await onBettingTimerElapsed(sess.Id);
        sess.TurnInfoChanged += async (sender, args) => await onTurnInfoChanged(sess.Id);
        
        await hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionCreated, sess.ToDto(getPlayerName));
    }

}