using System.Diagnostics;
using KC.Backend.API.Services;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Interfaces;
using KC.Frontend.Client.Services;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
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
        await hub.MoveToGroupAsync(connId, dto.SessionId.ToString());
        
        var sess = sessionLogic.Get(dto.SessionId);
        sess.DestructionTimer.Reset();
        
    }

    [HttpDelete]
    [Route("leave")]
    public async Task LeaveSession(SessionJoinLeaveDto dto)
    {
        var connId = playerLogic.Get(dto.Address).ConnectionId;
        await hub.MoveToGroupAsync(connId, "lobby");
    }

    private const uint DefaultBoxes = 5;
    private const uint DefaultDecks = 8;
    private const int DefaultShuffleCardPlacement = -40;
    private const uint DefaultShuffleCardRange = 10;
    private const uint DefaultBettingTimeSpanSecs = 15;
    private const uint DefaultSessionDestructionTimeSpanSecs = 10; 
    //private const uint DefaultSessionDestructionTimeSpanSecs = 5*60; // 5 minutes
    
    [HttpPost]
    [Route("create")]
    public void CreateSession()
    {
        var sess = sessionLogic.CreateSession(DefaultBoxes, DefaultDecks, DefaultShuffleCardPlacement, DefaultShuffleCardRange, TimeSpan.FromSeconds(DefaultBettingTimeSpanSecs), TimeSpan.FromSeconds(DefaultSessionDestructionTimeSpanSecs));
        sess.DestructionTimer.Elapsed += async (sender, args) => await OnDestructionTimerElapsed(sess.Id);
        hub.SendMessageToGroupAsync("lobby", "SessionCreated", sess.ToDto());
    }

    private async Task OnDestructionTimerElapsed(Guid id)
    {
        var destroyedSession = sessionLogic.RemoveSession(id);
        
        var connected = hub.ConnectionsAndGroups.Where(x => x.Value == id.ToString()).ToList();
        foreach (var conn in connected)
        {
            await hub.MoveToGroupAsync(conn.Key, "lobby");
        }
        await hub.SendMessageToGroupAsync("lobby", "SessionDeleted", id);

        foreach (var box in destroyedSession.Table.BettingBoxes)
        {
            if (box.OwnerId == MacAddress.None) continue;
            
            var p = playerLogic.Get(box.OwnerId);
            foreach (var hand in box.Hands)
            {
                if (hand.Bet > 0)
                    playerLogic.UpdateBalance(box.OwnerId, p.Balance+hand.Bet);
            }
            await hub.SendMessageAsync(p.ConnectionId, "PlayerBalanceUpdated", p.ToDto());
        }
    }
}