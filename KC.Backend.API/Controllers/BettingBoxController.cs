using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BettingBoxController(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic, IBetOrchestrator betOrchestrator, IClientCommunicator hub) : Controller
{
    [HttpPost]
    [Route("claim-box")]
    public void ClaimBox([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.ClaimBettingBox(dto.SessionId, dto.BoxIdx, MacAddress.Parse(macAddress));
        hub.SendMessageToGroupAsync(dto.SessionId.ToString(), SignalRMethods.BoxOwnerChanged, bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(g => playerLogic.Get(g).Name));
        hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionOccupancyChanged, (dto.SessionId,1));
    }

    [HttpDelete]
    [Route("disclaim-box")]
    public void DisclaimBox([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.DisclaimBettingBox(dto.SessionId, dto.BoxIdx, MacAddress.Parse(macAddress));
        hub.SendMessageToGroupAsync(dto.SessionId.ToString(), SignalRMethods.BoxOwnerChanged, bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(g => playerLogic.Get(g).Name));
        hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionOccupancyChanged, (dto.SessionId,-1));
    }

    [HttpPut]
    [Route("update-bet")]
    public async Task UpdateBet([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxBetUpdateDto dto) => await betOrchestrator.UpdateBet(MacAddress.Parse(macAddress), dto);
}