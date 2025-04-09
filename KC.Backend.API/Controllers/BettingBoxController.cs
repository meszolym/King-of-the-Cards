using KC.Backend.Logic.Interfaces;
using KC.Shared.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace KC.Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BettingBoxController(IBettingBoxLogic logic) : Controller
{
    [HttpPost]
    [Route("claim-box")]
    public void ClaimBox([FromBody] BoxOwnerUpdateDto dto) => logic.ClaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);
    
    [HttpDelete]
    [Route("disclaim-box")]
    public void DisclaimBox([FromBody] BoxOwnerUpdateDto dto) => logic.DisclaimBettingBox(dto.SessionId, dto.BoxIdx, dto.OwnerMac);
}