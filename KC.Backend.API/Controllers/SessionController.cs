using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, IPlayerLogic playerLogic, IHubContext<SignalRHub> hub) : Controller
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
        await hub.Groups.AddToGroupAsync(connId, dto.SessionId.ToString());
        await  hub.Groups.RemoveFromGroupAsync(connId, "lobby");
    }

    [HttpDelete]
    [Route("leave")]
    public async Task LeaveSession(SessionJoinLeaveDto dto)
    {
        var connId = playerLogic.Get(dto.Address).ConnectionId;
        await hub.Groups.RemoveFromGroupAsync(connId, dto.SessionId.ToString());
        await hub.Groups.AddToGroupAsync(connId, "lobby");
    }
}