using System.Diagnostics;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Logic.Services.Interfaces;
using KC.Backend.Models.GameManagement;
using KC.Frontend.Client.Services;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, ISessionCreationOrchestrator sessionCreationOrchestrator, IClientCommunicator hub) : Controller
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

    [HttpPost]
    [Route("create")]
    public async Task CreateSession() => await sessionCreationOrchestrator.CreateSession();

}