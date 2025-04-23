using KC.Backend.Logic.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BettingBoxController(IBettingBoxLogic bettingBoxLogic, IPlayerLogic playerLogic) : Controller
{
    [HttpPost]
    [Route("claim-box")]
    public void ClaimBox([FromBody] BoxOwnerUpdateDto dto) => bettingBoxLogic.ClaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);
    
    [HttpDelete]
    [Route("disclaim-box")]
    public void DisclaimBox([FromBody] BoxOwnerUpdateDto dto) => bettingBoxLogic.DisclaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);

    [HttpPut]
    [Route("update-bet")]
    public void UpdateBet([FromBody] BoxBetUpdateDto dto)
    {
        var player = playerLogic.Get(dto.OwnerMac);

        var alreadyPlaced = bettingBoxLogic.GetBetOnBox(dto.SessionId, dto.BoxIdx, dto.HandIdx);
        
        if (dto.Amount - alreadyPlaced > player.Balance)
            throw new InvalidOperationException("Player balance does not cover the bet.");
        
        bettingBoxLogic.UpdateBetOnBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac, dto.Amount, dto.HandIdx);
        playerLogic.UpdateBalance(dto.OwnerMac, player.Balance - (dto.Amount - alreadyPlaced));
        
    }
}