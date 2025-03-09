using KC.Backend.API.Utilities;
using KC.Backend.Logic.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic) : Controller
{
    [HttpGet]
    public IEnumerable<SessionDto> GetAllSessions() => sessionLogic.GetAll().Select(s => s.ToDto());

    [HttpGet("{id:guid}")]
    public SessionDto GetSession(Guid id) => sessionLogic.Get(id).ToDto();
}