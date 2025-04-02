using AutoMapper;
using KC.Backend.Logic.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController(ISessionLogic sessionLogic, Mapper mapper) : Controller
{
    [HttpGet]
    public IEnumerable<SessionReadDto> GetAllSessions() => sessionLogic.GetAll().Select(mapper.Map<SessionReadDto>);

    [HttpGet("{id:guid}")]
    public SessionReadDto GetSession(Guid id) => mapper.Map<SessionReadDto>(sessionLogic.Get(id));
    
    
}