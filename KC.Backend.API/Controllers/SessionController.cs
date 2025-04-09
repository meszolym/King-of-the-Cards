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
    public void JoinSession(SessionJoinLeaveDto dto) =>
        hub.Groups.AddToGroupAsync(playerLogic.Get(dto.Address).ConnectionId, dto.SessionId.ToString());

    [HttpDelete]
    [Route("leave")]
    public void LeaveSession(SessionJoinLeaveDto dto) =>
        hub.Groups.RemoveFromGroupAsync(playerLogic.Get(dto.Address).ConnectionId, dto.SessionId.ToString());
}