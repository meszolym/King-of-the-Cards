using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BettingBoxController(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic, IBetOrchestrator betOrchestrator, IClientCommunicator hub) : Controller
{
    [HttpPost]
    [Route("claim-box")]
    public void ClaimBox([FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.ClaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);
        hub.SendMessageToGroupAsync(dto.SessionId.ToString(), "BoxOwnerChanged", bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(g => playerLogic.Get(g).Name));
    }

    [HttpDelete]
    [Route("disclaim-box")]
    public void DisclaimBox([FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.DisclaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);
        hub.SendMessageToGroupAsync(dto.SessionId.ToString(), "BoxOwnerChanged", bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(g => playerLogic.Get(g).Name));
    }

    [HttpPut]
    [Route("update-bet")]
    public void UpdateBet([FromBody] BoxBetUpdateDto dto) => betOrchestrator.UpdateBet(dto);
}