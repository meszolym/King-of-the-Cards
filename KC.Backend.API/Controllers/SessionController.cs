using KC.Backend.API.Services;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Interfaces;
using KC.Frontend.Client.Services;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IClientCommunicator hub) : Controller
{
    
    
    [HttpGet]
    public IEnumerable<SessionReadDto> GetAllSessions() => sessionLogic.GetAll().Select(s => s.ToDto());

    [HttpGet("{id:guid}")]
    public SessionReadDto GetSession(Guid id) => sessionLogic.Get(id).ToDto();

    [HttpPost]
    [Route("join")]
    public async Task JoinSession(SessionJoinLeaveDto dto)
    {
        var connId = playerLogic.Get(dto.Address).ConnectionId;
        
        // await hub.Groups.AddToGroupAsync(connId, dto.SessionId.ToString());
        // await  hub.Groups.RemoveFromGroupAsync(connId, "lobby");
        
        await hub.RemoveFromGroupAsync(connId, "lobby");
        await hub.AddToGroupAsync(connId, dto.SessionId.ToString());
        
    }

    [HttpDelete]
    [Route("leave")]
    public async Task LeaveSession(SessionJoinLeaveDto dto)
    {
        var connId = playerLogic.Get(dto.Address).ConnectionId;
        
        // await hub.Groups.RemoveFromGroupAsync(connId, dto.SessionId.ToString());
        // await hub.Groups.AddToGroupAsync(connId, "lobby");
        
        await hub.RemoveFromGroupAsync(connId, dto.SessionId.ToString());
        await hub.AddToGroupAsync(connId, "lobby");
    }

    private const uint DefaultBoxes = 5;
    private const uint DefaultDecks = 8;
    private const int DefaultShuffleCardPlacement = -40;
    private const uint DefaultShuffleCardRange = 10;
    private const uint DefaultBettingTimeSpanSecs = 15;
    private const uint DefaultSessionDestructionTimeSpanSecs = 5*60; // 5 minutes
    
    [HttpPost]
    [Route("create")]
    public void CreateSession()
    {
        var sess = sessionLogic.CreateSession(DefaultBoxes, DefaultDecks, DefaultShuffleCardPlacement, DefaultShuffleCardRange, TimeSpan.FromSeconds(DefaultBettingTimeSpanSecs), TimeSpan.FromSeconds(DefaultSessionDestructionTimeSpanSecs));
        sess.DestructionTimer.Elapsed += (sender, args) => OnSessionDestruction(sess.Id);
        hub.SendMessageToGroupAsync("lobby", SignalRMethods.SessionCreated, sess.ToDto());
    }

    private void OnSessionDestruction(Guid id) =>
        hub.ConnectionsAndGroups.Where(x=> x.Value == id.ToString()).ToList().ForEach(async x =>
        {
            await hub.RemoveFromGroupAsync(x.Key, id.ToString());
            await hub.AddToGroupAsync(x.Key, "lobby");
        });
}