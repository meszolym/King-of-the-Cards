using System.Diagnostics;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
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
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, ISessionCreationOrchestrator sessionCreationOrchestrator, IClientCommunicator hub) : Controller
{
    [HttpGet]
    public IEnumerable<SessionReadDto> GetAllSessions() => sessionLogic.GetAll().Select(s => s.ToDto(g => playerLogic.Get(g).Name));

    [HttpGet("{id:guid}")]
    public SessionReadDto GetSession(Guid id) =>
        sessionLogic.Get(id).ToDto(g => playerLogic.Get(g).Name);

    [HttpPost("join/{sessionId:guid}")]
    public async Task JoinSession([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, Guid sessionId)
    {
        var mac = MacAddress.Parse(macAddress);
        var connId = playerLogic.Get(mac).ConnectionId;
        await hub.MoveToGroupAsync(connId, sessionId.ToString());
        
        var sess = sessionLogic.Get(sessionId);
        sess.DestructionTimer.Reset();
        
    }

    [HttpDelete("leave/{sessionId:guid}")]
    public async Task LeaveSession([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, Guid sessionId)
    {
        var connId = playerLogic.Get(MacAddress.Parse(macAddress)).ConnectionId;
        await hub.MoveToGroupAsync(connId, hub.BaseGroup);
    }

    [HttpPost]
    [Route("create")]
    public async Task CreateSession() => await sessionCreationOrchestrator.CreateSession();

}