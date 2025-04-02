using System.Net.NetworkInformation;
using AutoMapper;
using KC.Backend.Logic;
using KC.Backend.Logic.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(IPlayerLogic playerLogic, Mapper mapper) : Controller
{
    [HttpGet("{address}")]
    public PlayerReadDto GetPlayer(string address) => mapper.Map<PlayerReadDto>(playerLogic.Get(new MacAddress(address)));
    
    // [HttpPost]
    // public void AddPlayer([FromBody] PlayerDto playerDto) => playerLogic.AddPlayer(playerDto.ToModel());

    [HttpPost]
    public void AddPlayer([FromBody] PlayerRegisterDto dto) => playerLogic.AddPlayer(mapper.Map<Player>(dto));
}