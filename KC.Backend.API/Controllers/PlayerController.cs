using System.Net.NetworkInformation;
using KC.Backend.Logic;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(IPlayerLogic playerLogic) : Controller
{
    // [HttpGet("{address}")]
    // public PlayerReadDto GetPlayer(string address) => playerLogic.Get(MacAddress.Parse(address)).ToDto();
    
    [HttpGet]
    public PlayerReadDto GetPlayerByHeader([FromHeader(Name = "Player-Mac-Address")] string macAddress) => playerLogic.Get(MacAddress.Parse(macAddress)).ToDto();
    
    // [HttpPost]
    // public void AddPlayer([FromBody] PlayerDto playerDto) => playerLogic.AddPlayer(playerDto.ToModel());

    [HttpPost("{name}")] 
    public void AddPlayer([FromHeader(Name = "Player-Mac-Address")] string macAddress, string name) => playerLogic.AddPlayer(MacAddress.Parse(macAddress), new Player(){Name = name});

    [HttpPut("update-conn-id/{connectionId}")]
    public void UpdatePlayerConnectionId([FromHeader(Name = "Player-Mac-Address")] string macAddress, string connectionId) => playerLogic.UpdatePlayerConnectionId(MacAddress.Parse(macAddress), connectionId);
}