using KC.Backend.API.Extensions;
using KC.Backend.API.Services;
using KC.Backend.API.Services.Interfaces;
using KC.Backend.Logic;
using KC.Backend.Logic.Extensions;
using KC.Backend.Logic.Logics.Interfaces;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.Misc;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BettingBoxController(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic, ISessionLogic sessionLogic, IClientCommunicator hub, GetPlayerNameDelegate getPlayerName, BetUpdatedDelegate betUpdated) : Controller
{
    [HttpPost]
    [Route("claim-box")]
    public void ClaimBox([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.ClaimBettingBox(dto.SessionId, dto.BoxIdx, MacAddress.Parse(macAddress));
        hub.SendMessageToGroupAsync(dto.SessionId, SignalRMethods.BoxOwnerChanged, bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(getPlayerName));
        hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionOccupancyChanged, (dto.SessionId,1));
    }

    [HttpDelete]
    [Route("disclaim-box")]
    public void DisclaimBox([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxOwnerUpdateDto dto)
    {
        bettingBoxLogic.DisclaimBettingBox(dto.SessionId, dto.BoxIdx, MacAddress.Parse(macAddress));
        hub.SendMessageToGroupAsync(dto.SessionId, SignalRMethods.BoxOwnerChanged, bettingBoxLogic.Get(dto.SessionId, dto.BoxIdx).ToDto(getPlayerName));
        hub.SendMessageToGroupAsync(hub.BaseGroup, SignalRMethods.SessionOccupancyChanged, (dto.SessionId,-1));
    }

    [HttpPut]
    [Route("update-bet")]
    //TODO: Maybe move to logic level?
    public async Task UpdateBet([FromHeader(Name = HeaderNames.PlayerMacAddress)] string macAddress, [FromBody] BoxBetUpdateDto dto)
    {
        var address = MacAddress.Parse(macAddress);
        var player = playerLogic.Get(address);

        var alreadyPlaced = bettingBoxLogic.GetBetOnBox(dto.SessionId, dto.BoxIdx, dto.HandIdx);
        
        if (dto.Amount - alreadyPlaced > player.Balance)
            throw new InvalidOperationException("Player balance does not cover the bet.");
        
        bettingBoxLogic.UpdateBetOnBox(dto.SessionId, dto.BoxIdx, address, dto.Amount, dto.HandIdx);
        playerLogic.UpdateBalance(address, player.Balance - (dto.Amount - alreadyPlaced));
        
        await hub.SendMessageAsync(player.ConnectionId, SignalRMethods.PlayerBalanceUpdated, player.ToDto());
        await betUpdated(dto.SessionId, dto.BoxIdx);
        
        var running = sessionLogic.UpdateBettingTimer(dto.SessionId);

        if (!running)
            await hub.SendMessageToGroupAsync(dto.SessionId, SignalRMethods.BettingTimerStopped, dto.SessionId);
    }
}