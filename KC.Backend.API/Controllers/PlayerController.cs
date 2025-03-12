using System.Net.NetworkInformation;
using KC.Backend.Logic;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(PlayerLogic playerLogic) : Controller
{
    [HttpGet]
    public Player GetPlayer([FromBody] MacAddress id) => playerLogic.Get(id);
    
    // [HttpPost]
    // public void AddPlayer([FromBody] PlayerDto playerDto) => playerLogic.AddPlayer(playerDto.ToModel());
}